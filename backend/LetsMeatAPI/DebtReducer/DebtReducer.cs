using LetsMeatAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LetsMeatAPI {
  public interface IDebtReducer {
    public Task ReduceDebts(Guid groupId);
  }
  public class DebtReducer : IDebtReducer {
    public DebtReducer(
      LMDbContext context,
      ILogger<DebtReducer> logger
    ) {
      _context = context;
      _logger = logger;
    }
    public async Task ReduceDebts(Guid groupId) {
      using var transaction = await _context.Database.BeginTransactionAsync(
        IsolationLevel.Serializable
      );
      var debtsToBeReduced = GroupDebtsToDict(groupId);
      _context.RemoveRange(_context.Debts.Where(d => d.GroupId == groupId));
      ReduceCycles(debtsToBeReduced);
      ReducePaths(debtsToBeReduced);
      await DumpDebtMapToDb(groupId, debtsToBeReduced);
      try {
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
      } catch(Exception ex) {
        _logger.LogError(ex.ToString());
        // Reduction failure is not fatal
      }
    }
    private IEnumerable<string>? FindCycle(Dictionary<string, Dictionary<string, int>> debtMap) {
      var visited = new HashSet<string>();
      IEnumerable<string>? dfs(string current) {
        if(!debtMap.ContainsKey(current))
          return null;
        visited.Add(current);
        foreach(var id in debtMap[current].Keys) {
          if(visited.Contains(id)) {
            // Found a cycle!
            visited.Remove(current);
            return Enumerable.Empty<string>().Append(id).Append(current);
          }
          var ret = dfs(id);
          if(ret != null) {
            visited.Remove(current);
            return ret.Append(current);
          }
        }
        visited.Remove(current);
        return null;
      }
      foreach(var id in debtMap.Keys) {
        var ret = dfs(id);
        if(ret != null)
          return ret.Reverse();
      }
      return null;
    }
    private IEnumerable<string>? FindPathSuitableForReduction(Dictionary<string, Dictionary<string, int>> debtMap) {
      IEnumerable<IEnumerable<string>> dfs(string current) {
        if(!debtMap.ContainsKey(current)) {
          yield return Enumerable.Empty<string>().Append(current);
          yield break;
        }
        foreach(var id in debtMap[current].Keys) {
          foreach(var ret in dfs(id))
            yield return ret.Append(current);
        }
      }
      var startpoints = debtMap.Where(p => debtMap.All(pp => !pp.Value.ContainsKey(p.Key)))
                               .Select(p => p.Key);
      IEnumerable<string>? path = null;
      var pathLength = 0;
      foreach(var p in startpoints) {
        foreach(var pp in dfs(p)) {
          var ppLength = pp.Count();
          if(path == null || pathLength < ppLength)
            (path, pathLength) = (pp, ppLength);
        }
      }
      if(pathLength < 3) // At least 2 edges are necessary
        return null;
      return path!.Reverse();
    }
    private void SimplifyDebtMap(Dictionary<string, Dictionary<string, int>> debtMap) {
      foreach(var toSet in debtMap.Select(p => p.Value)) {
        foreach(var id in toSet.Where(p => p.Value == 0)
                               .Select(p => p.Key)
                               .ToArray()) {
          toSet.Remove(id);
        }
      }
      foreach(var id in debtMap.Where(p => p.Value.Count() == 0)
                               .Select(p => p.Key)
                               .ToArray()) {
        debtMap.Remove(id);
      }
    }
    private void ReduceCycles(Dictionary<string, Dictionary<string, int>> debtMap) {
      IEnumerable<string>? cycle;
      while((cycle = FindCycle(debtMap)) != null) {
        var cycleArray = cycle.ToArray();
        var cycleEdges = cycleArray.Zip(cycleArray.Skip(1)).ToArray();
        var cycleWeight = cycleEdges.Min(
          p => debtMap[p.First][p.Second]
        );
        foreach(var (v, w) in cycleEdges)
          debtMap[v][w] -= cycleWeight;
        SimplifyDebtMap(debtMap);
      }
    }
    private void ReducePaths(Dictionary<string, Dictionary<string, int>> debtMap) {
      IEnumerable<string>? path;
      while((path = FindPathSuitableForReduction(debtMap)) != null) {
        var pathArray = path.ToArray();
        var begin = pathArray[0];
        var end = pathArray[^1];
        var pathEdges = pathArray.Zip(pathArray.Skip(1)).ToArray();
        var pathWeight = pathEdges.Min(
          p => debtMap[p.First][p.Second]
        );
        foreach(var (v, w) in pathEdges)
          debtMap[v][w] -= pathWeight;
        var d = debtMap[begin];
        d[end] = (d.ContainsKey(end) ? d[end] : 0) + pathWeight;
        SimplifyDebtMap(debtMap);
      }
    }
    private Dictionary<string, Dictionary<string, int>> GroupDebtsToDict(Guid groupId) {
      var ret = new Dictionary<string, Dictionary<string, int>>();
      foreach(var debt in from debt in _context.Debts
                          where debt.GroupId == groupId && debt.Amount != 0
                          select debt) {
        if(debt.Amount > 0) {
          if(!ret.ContainsKey(debt.FromId))
            ret[debt.FromId] = new();
          ret[debt.FromId][debt.ToId] = debt.Amount;
        } else {
          if(!ret.ContainsKey(debt.ToId))
            ret[debt.ToId] = new();
          ret[debt.ToId][debt.FromId] = -debt.Amount;
        }
      }
      return ret;
    }
    private async Task DumpDebtMapToDb(Guid groupId, Dictionary<string, Dictionary<string, int>> debtMap) {
      foreach(var (from, toSet) in debtMap) {
        foreach(var (to, amount) in toSet) {
          var debt = new Debt {
            Amount = amount,
            FromId = from,
            ToId = to,
            GroupId = groupId,
          };
          if(from.CompareTo(to) < 0)
            (debt.FromId, debt.ToId, debt.Amount) = (debt.ToId, debt.FromId, -debt.Amount);
          await _context.Debts.AddAsync(debt);
        }
      }
    }
    private readonly LMDbContext _context;
    private readonly ILogger<DebtReducer> _logger;
  }
}
