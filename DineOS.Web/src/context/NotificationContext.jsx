import React, { createContext } from 'react';
import { toast } from 'react-toastify';

export const NotificationContext = createContext();

export const NotificationProvider = ({ children }) => {
  const showSuccess = (message) => {
    toast.success(message, {
      position: 'top-right',
      autoClose: 3000,
    });
  };

  const showError = (message) => {
    toast.error(message, {
      position: 'top-right',
      autoClose: 3000,
    });
  };

  const showWarning = (message) => {
    toast.warning(message, {
      position: 'top-right',
      autoClose: 3000,
    });
  };

  const showInfo = (message) => {
    toast.info(message, {
      position: 'top-right',
      autoClose: 3000,
    });
  };

  return (
    <NotificationContext.Provider
      value={{
        showSuccess,
        showError,
        showWarning,
        showInfo,
      }}
    >
      {children}
    </NotificationContext.Provider>
  );
};