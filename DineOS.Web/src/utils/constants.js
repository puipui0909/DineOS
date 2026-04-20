export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5010/api';

export const USER_ROLES = {
  ADMIN: 'admin',
  STAFF: 'staff',
  CUSTOMER: 'customer',
};

export const ORDER_STATUS = {
  PENDING: 'pending',
  CONFIRMED: 'confirmed',
  PREPARING: 'preparing',
  READY: 'ready',
  COMPLETED: 'completed',
  CANCELLED: 'cancelled',
};

export const TABLE_STATUS = {
  EMPTY: 'empty',
  OCCUPIED: 'occupied',
  RESERVED: 'reserved',
};

export const PAYMENT_METHOD = {
  CASH: 'cash',
  ZALOPAY: 'zalopay',
  CARD: 'card',
};