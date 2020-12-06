import { url as base } from './url'

export const getGroups = id => {
  const url = base + '/Groups?userId=' + id

  return new Promise((resolve, reject) => {
    resolve([
      { id: 0, name: 'Obiadki', date: '12.02.20' },
      { id: 1, name: 'Picie!', date: '01.03.20' },
      { id: 2, name: 'Kulturki', date: '19.02.20' },
      { id: 3, name: 'Brawurki', date: '22.02.20' },
      { id: 4, name: 'Obiadki praca', date: '12.03.20' },
      { id: 5 ,name: 'Obiadki u twojej mamy', date: 'dzisiaj' },
    ])
  })
}
