import axios from 'axios';

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/table`;

export const tableService = {
  getAll: () => axios.get(API_URL),
  create: (data) => axios.post(API_URL, data),
  createMultiple: (data) => axios.post(`${API_URL}/bulk`, data),

  updateStatus: (id, status) =>
  axios.patch(`${API_URL}/${id}/status`, status, {
    headers: {
      "Content-Type": "application/json"
    }
  }),

  delete: (id) => axios.delete(`${API_URL}/${id}`),

};