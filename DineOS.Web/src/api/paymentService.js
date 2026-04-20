import axiosInstance from './axiosConfig';

export const paymentService = {
  pay: async (orderId, method) => {
    const res = await axiosInstance.post(`/payment/${orderId}/pay`, {
      method: method.toUpperCase(),
    });
    return res.data;
  },

  getByOrder: async (orderId) => {
    const res = await axiosInstance.get(`/payment/order/${orderId}`);
    return res.data;
  },

  getById: async (paymentId) => {
    const res = await axiosInstance.get(`/payment/${paymentId}`);
    return res.data;
  },
};