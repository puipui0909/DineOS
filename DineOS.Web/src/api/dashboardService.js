import axiosInstance from './axiosConfig';

export const dashboardService = {
  getSummary: async () => {
    const res = await axiosInstance.get('/dashboard/summary');
    return res.data;
  },

  getRevenueMonthly: async (year) => {
    const res = await axiosInstance.get(`/dashboard/revenue-monthly?year=${year}`);
    return res.data;
  }
};