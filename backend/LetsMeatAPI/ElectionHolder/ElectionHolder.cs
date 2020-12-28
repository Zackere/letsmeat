using System;
using System.Collections.Generic;
using System.Linq;

namespace LetsMeatAPI {
  public interface IElectionHolder {
    public IEnumerable<int> DecideWinner(int nCandidates, IEnumerable<IEnumerable<int>> votes);
  }
  public class ElectionHolder : IElectionHolder {
    public IEnumerable<int> DecideWinner(int nCandidates, IEnumerable<IEnumerable<int>> votes) {
      if(votes.Any(v => v.Any(vv => !(vv >= 0 && vv < nCandidates))))
        throw new ArgumentException();
      if(votes.Any(v => v.Distinct().Count() != nCandidates))
        throw new ArgumentException();
      return DecideWinnerImpl(nCandidates, votes).Reverse();
    }
    private IEnumerable<int> DecideWinnerImpl(
      int nCandidates,
      IEnumerable<IEnumerable<int>> votes
    ) {
      var candidates = Enumerable.Range(0, nCandidates).ToArray();
      var votesCount = new int[nCandidates];
      while(candidates.Any()) {
        Array.Fill(votesCount, 0);
        foreach(var vote in votes)
          ++votesCount[vote.First()];
        var minVotes = candidates.Min(c => votesCount[c]);
        var toBeRemoved = candidates.Where(c => votesCount[c] == minVotes).ToArray();
        votes = votes.Select(v => v.Except(toBeRemoved)).ToArray();
        candidates = candidates.Except(toBeRemoved).ToArray();
        foreach(var c in toBeRemoved)
          yield return c;
      }
    }
  }
}
