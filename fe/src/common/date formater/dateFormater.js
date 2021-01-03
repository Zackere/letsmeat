export const basicFormat = dateString => {

  const date = new Date(dateString)
  let minutes = date.getMinutes()
  let hours = date.getHours()

  if (minutes < 10) minutes = '0' + minutes
  if (hours < 10) hours = '0' + hours

  return (
    weekDays(date.getDay()) +
    ', ' +
    date.getDate() +
    ' ' +
    month(date.getMonth()) +
    ', ' +
    hours +
    '.' +
    minutes
  )
}

const weekDays = number => {
  switch (number) {
    case 1:
      return 'Mon.'
    case 2:
      return 'Tue.'
    case 3:
      return 'Wed.'
    case 4:
      return 'Thu.'
    case 5:
      return 'Fri.'
    case 6:
      return 'Sat.'
    default:
      return 'Sun.'
  }
}

const month = number => {
  switch (number) {
    case 0:
      return 'Jan.'
    case 1:
      return 'Feb.'
    case 2:
      return 'Mar.'
    case 3:
      return 'Apr.'
    case 4:
      return 'May'
    case 5:
      return 'Jun.'
    case 6:
      return 'Jul.'
    case 7:
      return 'Aug.'
    case 8:
      return 'Sep.'
    case 9:
      return 'Oct.'
    case 10:
      return 'Nov.'
    default:
      return 'Dec.'
  }
}
