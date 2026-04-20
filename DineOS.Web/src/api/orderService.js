import axiosInstance from './axiosConfig';

export const orderService = {
  staff: {
    getByTable: async (tableId) => {
      const res = await axiosInstance.get(`/order/table/${tableId}`);
      return res.data;
    },

    getById: async (orderId) => {
      const res = await axiosInstance.get(`/order/${orderId}`);
      return res.data;
    },

    getAll: async () => {
      const res = await axiosInstance.get('/order');
      return res.data;
    },

    addItem: async (orderId, menuItemId, quantity) => {
      const res = await axiosInstance.post(`/order/${orderId}/items`, {
        menuItemId,
        quantity,
      });
      return res.data;
    },

    removeItem: async (orderId, orderItemId) => {
      return await axiosInstance.delete(`/order/${orderId}/items/${orderItemId}`);
    },

    closeOrder: async (orderId) => {
      const res = await axiosInstance.patch(`/order/${orderId}/close`);
      return res.data;
    },

    cancelOrder: async (orderId) => {
      const res = await axiosInstance.post(`/order/${orderId}/cancel`);
      return res.data;
    },

    getBill: async (orderId) => {
      const res = await axiosInstance.get(`/order/${orderId}/bill`);
      return res.data;
    },

    getHistory: async ({ search, fromDate, toDate }) => {
      const params = {};

      if (search) params.search = search;
      if (fromDate) params.fromDate = fromDate;
      if (toDate) params.toDate = toDate;

      const res = await axiosInstance.get('/order/history', { params });
      return res.data;
    },

    sendToKitchen: async (orderId) => {
      const res = await axiosInstance.patch(`/order/${orderId}/send-to-kitchen`);
      return res.data;
    },
  },
  
  customer: {
    createOrder: async (payload) => {
      const res = await axiosInstance.post('/customer/orders/checkout', payload);
      return res.data;
    },

    getByTable: async (tableId) => {
      const res = await axiosInstance.get(`/customer/orders/by-table/${tableId}`);
      return res.data;
    },

    addItem: async (tableId, menuItemId, quantity) => {
      const res = await axiosInstance.post(`/customer/orders/by-table/${tableId}/items`, {
        menuItemId,
        quantity,
      });
      return res.data;
    },

    sendToKitchen: async (tableId) => {
      const res = await axiosInstance.post(`/customer/orders/by-table/${tableId}/send-to-kitchen`);
      return res.data;
    }
  }
  
};