export const formatAmount = (amount) => (amount / 100).toFixed(2);

export const parseAmount = (amount) => 100 * parseFloat(amount);
