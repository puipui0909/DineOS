import axiosInstance from './axiosConfig';
import axiosPublic from './axiosPublic';

export const menuService = {
  getAll: async () => {
    const res = await axiosInstance.get('/menu/GetAll');
    return res.data;
  },

  getByCategory: async (categoryId) => {
    const res = await axiosInstance.get(`/menu/GetCategory/${categoryId}`);
    return res.data;
  },

  getById: async (id) => {
    const res = await axiosInstance.get(`/menu/GetbyID/${id}`);
    return res.data;
  },

  create: async (data) => {
    const res = await axiosInstance.post('/menu/CreateMenuItem', data);
    return res.data;
  },

  upload: async (formData) => {
    const res = await axiosInstance.post('/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return res.data;
  },

  update: async (id, data) => {
    const res = await axiosInstance.put(`/menu/UpdateItem/${id}`, data);
    return res.data;
  },

  delete: async (id) => {
    const res = await axiosInstance.delete(`/menu/DeletaItem/${id}`);
    return res.data;
  },
  toggleStatus: async (id) => {
    await axiosInstance.patch(`/menu/toggle/${id}`);
  },

  getByTable: async (tableId) => {
    const res = await axiosPublic.get('/menu/by-table', {
      params: { tableId }
    });
    return res.data;
  }
};