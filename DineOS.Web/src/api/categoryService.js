import axiosInstance from './axiosConfig';
import axiosPublic from './axiosPublic';

export const categoryService = {
    getAll: async () => {
      const res = await axiosInstance.get('/Category/GetAll');
      return res.data;
    },
    create: async (data) => {
      const res = await axiosInstance.post('/Category', data);
      return res.data;
    },

    getAllPublic: async (tableId) => {
      const res = await axiosPublic.get(`/Category/customer/by-table/${tableId}`);
      return res.data;
    }
};