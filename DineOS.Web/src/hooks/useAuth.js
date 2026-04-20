import authService from '../api/authService';

export const useAuth = () => {
  const user = authService.getUserInfo();

  const login = async (username, password) => {
    return await authService.login(username, password);
  };

  const logout = async () => {
    return await authService.logout();
  };

  return {
    user,
    role: user?.role,
    isAdmin: user?.role === 'Admin',
    isStaff: user?.role === 'Staff',
    login,    
    logout,    
  };
};