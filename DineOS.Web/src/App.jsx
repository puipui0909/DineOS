import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { OrderProvider } from './context/OrderContext';
import { NotificationProvider } from './context/NotificationContext';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

//Pages
import Login from './pages/login/Login';
import Dashboard from './pages/admin/dashboard/DashboardPage';
import Menu from './pages/admin/menu/Menu';
import Table from './pages/admin/table/Table';
import MainLayout from './pages/admin/MainLayout';
import OrderRailPage from './pages/admin/order/rail/OrderRailPage';
import OrderPage from './pages/admin/order/OrderPage';
import OrderHistoryPage from './pages/admin/history/OrderHistoryPage';
import CustomerPage from "./pages/customer/CustomerPage";

function App() {
  return (
    <AuthProvider>
      <OrderProvider>
        <NotificationProvider>  
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/" element={<Navigate to="/login" replace />} />

            <Route path="/admin" element={<MainLayout />}>
              <Route index element={<Navigate to="dashboard" replace />} />
              <Route path="dashboard" element={<Dashboard />} />
              <Route path="menu" element={<Menu />} />
              <Route path="table" element={<Table />} />
              <Route path="orderrail" element={<OrderRailPage />} />
              <Route path="order/:tableId" element={<OrderPage />} />
              <Route path="history" element={<OrderHistoryPage />} />
            </Route>
            <Route path="/customer" element={<CustomerPage />} />
          </Routes>
          <ToastContainer />
        </NotificationProvider>
      </OrderProvider>
    </AuthProvider>
  );
}

export default App;