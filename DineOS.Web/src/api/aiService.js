import axios from 'axios';
import { API_BASE_URL } from '../utils/constants';

const API_URL = `${API_BASE_URL}/ai`;

export const aiService = {
  suggest: async (message) => {
    const res = await axios.post(`${API_URL}/suggest`, {
      message
    });
    return res.data;
  }
};