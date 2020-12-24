export const months = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December'
];

export const days = [
  'Sun',
  'Mon',
  'Tue',
  'Wed',
  'Thu',
  'Fri',
  'Sat'
];

export const formatTime = (time) => (`${time.getHours().toString().padStart(2, '0')}:${time.getMinutes().toString().padStart(2, '0')}`);

export const formatDate = (time) => {
  const year = time.getFullYear(); // 2019
  const date = time.getDate(); //
  const monthName = months[time.getMonth()];
  const dayName = days[time.getDay()]; // Thu
  return `${dayName}, ${date} ${monthName} ${year}`;
};

export const getTimeLeft = (timePoint, includeSeconds = false) => {
  const now = new Date();
  const msDiff = timePoint - now;
  const ms = Math.abs(msDiff);
  const days = Math.floor(ms / (1000 * 60 * 60 * 24));
  const hours = Math.floor(ms / (1000 * 60 * 60)) - 24 * days;
  const minutes = Math.floor(ms / (1000 * 60)) - 24 * 60 * days - 60 * hours;
  const seconds = Math.floor(ms / 1000) - 24 * 60 * 60 * days - 60 * 60 * hours - 60 * minutes;
  return `${days} days, ${hours} hours ${minutes} minutes ${includeSeconds ? `${seconds} seconds` : ''}${msDiff <= 0 ? 'ago' : 'left'}`;
};
