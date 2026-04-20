import { jwtDecode } from 'jwt-decode';
import Cookies from 'js-cookie';
import axiosInstance from './axiosConfig';

class AuthService {
  constructor() {
    this.accessToken = null;
    this.refreshToken = null;
    this.userInfo = null;
  }

  isTokenExpired(token) {
    if (!token) return true;
    try {
      const decoded = jwtDecode(token);
      const currentTime = Date.now() / 1000;
      return decoded.exp - currentTime < 300; // Refresh nếu < 5 phút
    } catch {
      return true;
    }
  }

  getAccessToken() {
    return this.accessToken || localStorage.getItem("accessToken");
  }

  async login(username, password) {
    try {
      console.log("LOGIN DATA:", {
        username,
        password
      });
      const response = await axiosInstance.post(
        '/auth/login',
        JSON.stringify({
          username: username,
          password: password,
        }),
        {
          headers: {
            'Content-Type': 'application/json',
          },
        }
      );
      console.log("RESPONSE:", response.data);

      const { token, username: user } = response.data;
      if (!token) {
      throw 'Không nhận được token từ server';
      }
      this.accessToken = token;

      let decoded;
      try {
        decoded = jwtDecode(token);
        console.log("DECODED TOKEN:", decoded);
      } catch (err) {
        console.log("DECODE ERROR:", err);
        throw 'Token không hợp lệ';
      }
      

      this.userInfo = {
        username: user,
        role:
          decoded.role ||
          decoded.roles ||
          decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
          null,
        restaurantId: decoded.restaurantId || null
      };

      localStorage.setItem('user', JSON.stringify(this.userInfo));
      localStorage.setItem('accessToken', token);

      return this.userInfo;
    } catch (error) {
    console.log('FULL ERROR:', error);
      if (error.response) {
        throw error.response.data?.message || 'Sai tài khoản hoặc mật khẩu';
      }

      if (error.request) {
        throw 'Không kết nối được server';
      }

      throw error.message || 'Lỗi không xác định';
    }
  }

  async refreshAccessToken() {
    try {
      // Giả sử backend hỗ trợ refresh endpoint
      const response = await axiosInstance.post('/auth/refresh', {});
      const { token } = response.data;
      this.accessToken = token;
      localStorage.setItem('accessToken', token);
      return token;
    } catch (error) {
      this.logout();
      throw error;
    }
  }

  logout() {
    this.accessToken = null;
    this.refreshToken = null;
    this.userInfo = null;

    localStorage.removeItem('accessToken');
    localStorage.removeItem('user');
  }

  isAuthenticated() {
    const token = this.accessToken || localStorage.getItem("accessToken");
    return !!token && !this.isTokenExpired(token);
  }

  getUserInfo() {
    if (!this.userInfo) {
      const stored = localStorage.getItem('user');
      this.userInfo = stored ? JSON.parse(stored) : null;
    }
    return this.userInfo;
  }

  initializeFromStorage() {
    const token = localStorage.getItem('accessToken');
    const user = localStorage.getItem('user');
    if (token) {
      this.accessToken = token;
    }
    if (user) {
      this.userInfo = JSON.parse(user);
    }
  }
}

export default new AuthService();