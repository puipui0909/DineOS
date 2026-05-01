import React, { useEffect, useState, useCallback } from 'react';
import { Menu, MenuItem } from '@mui/material';
import { useNavigate, useLocation, NavLink, Outlet } from 'react-router-dom';
import { toast, ToastContainer } from 'react-toastify';

import { useAuth } from '../../hooks/useAuth';
import { startOrderRealtime, stopOrderRealtime, connection } from '../../api/orderRealtime';

import { 
  Box, Drawer, AppBar, Toolbar, List, Typography, 
  Divider, ListItem, ListItemButton, ListItemIcon, ListItemText, IconButton, Avatar,
  Badge, Menu as NotificationMenu, Button
} from '@mui/material';
import { 
  Dashboard as DashboardIcon, 
  ShoppingCart as OrderIcon, 
  RestaurantMenu as MenuIcon, 
  History as HistoryIcon, 
  Settings as SettingsIcon,
  Notifications as NotificationsIcon,
  TableRestaurant as TableIcon,
} from '@mui/icons-material';

const drawerWidth = 240;

const MainLayout = () => {
  const { role, user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [notifications, setNotifications] = useState([]);
  const orderItemCountRef = React.useRef({});
  const [notiAnchorEl, setNotiAnchorEl] = useState(null);
  
  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const [anchorEl, setAnchorEl] = useState(null);

  const open = Boolean(anchorEl);

  const handleAvatarClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };
  // Danh sách các nút điều hướng trên Sidebar
  const menuItems = [
    { text: 'Dashboard', icon: <DashboardIcon />, path: '/admin/dashboard', roles: ['Admin', 'Staff'] },
    { text: 'Menu', icon: <MenuIcon />, path: '/admin/menu', roles: ['Admin', 'Staff'] },
    { text: 'Orders', icon: <OrderIcon />, path: '/admin/orderrail', roles: ['Admin', 'Staff'] },
    { text: 'Tables', icon: <TableIcon />, path: '/admin/table', roles: ['Admin', 'Staff'] },
    { text: 'History', icon: <HistoryIcon />, path: '/admin/history', roles: ['Admin', 'Staff'] },
  ];

  const audioRef = React.useRef(new Audio("/sounds/notification.mp3"));
  const getTotalQuantity = (items) => {
    if (!items) return 0;
    return items.reduce((sum, i) => sum + (i.quantity || 0), 0);
  };

  const startedRef = React.useRef(false);

  const handleOrderUpdate = useCallback((updatedOrder) => {
    console.log("🔄 MAIN LAYOUT RECEIVE:", updatedOrder.id);
    const isAtOrderRail = location.pathname === '/admin/orderrail';

    const tableId = updatedOrder.table?.id;
    const tableName = updatedOrder.table?.name ?? "N/A";
    const orderId = updatedOrder.id;
    const newCount = getTotalQuantity(updatedOrder.items);
    const prevCount = orderItemCountRef.current[orderId];

    if (prevCount !== undefined && newCount > prevCount) {
      if (!isAtOrderRail) {
        setNotifications(prev => {
            if (prev.some(n => n.tableId === tableId)) return prev;
            return [{ id: tableId, tableId, tableName, time: new Date(), isNewUpdate: true }, ...prev];
        });
      }
      audioRef.current.currentTime = 0;
      audioRef.current.play().catch(() => {});
      toast.info(`Bàn ${tableName} vừa thêm món mới!`);
    }
    orderItemCountRef.current[orderId] = newCount;
  }, [location.pathname]);

  const handleOrderCreated = useCallback((newOrder) => {
    const isAtOrderRail = location.pathname === '/admin/orderrail';
    const tableId = newOrder.table?.id;
    const tableName = newOrder.table?.name ?? "N/A";
    if (!isAtOrderRail) {
        setNotifications(prev => [{ id: tableId, tableId, tableName, time: new Date(), isNewUpdate: false }, ...prev]);
    }
    audioRef.current.play().catch(() => {});
    toast.info(`Bàn ${tableName} vừa tạo đơn mới`);
  }, [location.pathname]);
  const clearNotificationsByTable = useCallback((tableId) => {
    // 1. Xóa trong Menu (State)
    setNotifications(prev => prev.filter(n => n.tableId !== tableId));
    
    // 2. Xóa box Toast nổi trên màn hình
    toast.dismiss(tableId);
  }, [location.pathname]);

  useEffect(() => {
    startOrderRealtime(handleOrderCreated, handleOrderUpdate);
    // Cleanup khi component unmount
    return () => {
      stopOrderRealtime(handleOrderCreated, handleOrderUpdate);
    };
  }, [handleOrderCreated, handleOrderUpdate]);

  useEffect(() => {
    if (location.pathname === '/admin/orderrail') {
      // 1. Xóa sạch danh sách trong Badge và Menu chuông ngay lập tức
      setNotifications([]); 
      
      // 2. Xóa toàn bộ các box Toast đang hiện trên màn hình
      toast.dismiss();      
      
      console.log("🧹 Đã dọn sạch thông báo khi vào trang Orders");
    }
  }, [location.pathname]); // Chạy mỗi khi chuyển trang

  const handleNotiClick = (event) => setNotiAnchorEl(event.currentTarget);
  const handleNotiClose = () => setNotiAnchorEl(null);

  return (
    <Box sx={{ display: 'flex' }}>
      {/* 1. HEADER (Top Bar) */}
      <AppBar 
        position="fixed" 
        sx={{ 
          zIndex: (theme) => theme.zIndex.drawer + 1,
          width: `calc(100% - ${drawerWidth}px)`, 
          ml: `${drawerWidth}px`,
          bgcolor: 'white',
          color: 'text.primary',
          boxShadow: 'none',
          display: 'flex',
          borderBottom: '1px solid #ddd'
        }}
      >
        <Toolbar sx={{ justifyContent: 'space-between' }}>
          <Typography variant="h6" noWrap component="div" sx={{ fontWeight: 'bold' }}>
            Quản lý nhà hàng DineOS
          </Typography>

          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <IconButton color="inherit" onClick={handleNotiClick}>
              <Badge badgeContent={notifications.length} color="error">
                <NotificationsIcon />
              </Badge>
            </IconButton>
            <NotificationMenu
              anchorEl={notiAnchorEl}
              open={Boolean(notiAnchorEl)}
              onClose={handleNotiClose}
              PaperProps={{ sx: { width: 300, maxHeight: 400, mt: 1 } }}
            >
              <Box sx={{ p: 2, borderBottom: '1px solid #eee' }}>
                <Typography fontWeight="bold">Đơn hàng mới chưa xem</Typography>
              </Box>

              {notifications.length === 0 ? (
                <MenuItem sx={{ py: 2, color: 'gray', justifyContent: 'center' }}>
                  Không có thông báo mới
                </MenuItem>
              ) : (
                notifications.map((noti) => (
                  <MenuItem 
                    key={noti.id} 
                    onClick={() => {
                      handleNotiClose(); 
                      // XOÁ thông báo này khỏi danh sách sau khi click
                      setNotifications(prev => prev.filter(n => n.id !== noti.id)); 
                      navigate('/admin/orderrail');
                    }}
                    sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start' }}
                  >
                    <Typography variant="body2">
                      Khách tại <b>Bàn {noti.tableName}</b> đã đặt món.
                    </Typography>
                    <Typography variant="caption" color="primary">
                      {noti.time.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                    </Typography>
                  </MenuItem>
                ))
              )}
              
              {notifications.length > 0 && (
                <Box sx={{ p: 1, textAlign: 'center' }}>
                  <Button size="small" onClick={() => navigate('/admin/orderrail')}>
                    Xem tất cả trên Rail
                  </Button>
                </Box>
              )}
            </NotificationMenu>

            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <Typography variant="body2">
                {user?.username}
              </Typography>

              <Avatar 
                sx={{ bgcolor: '#1976d2', cursor: 'pointer' }}
                onClick={handleAvatarClick}
              >
                {user?.username?.[0]?.toUpperCase()}
              </Avatar>

              <Menu
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
              >
                <MenuItem onClick={async () => {
                  handleClose();
                  await logout();
                  navigate('/login');
                }}>
                  Đăng xuất
                </MenuItem>
              </Menu>
            </Box>

          </Box>
        </Toolbar>
      </AppBar>

      {/* 2. SIDEBAR (Drawer) */}
      <Drawer
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: drawerWidth,
            boxSizing: 'border-box',
            bgcolor: '#1a2035',
            color: 'white'
          },
        }}
        variant="permanent"
        anchor="left"
      >
        <Toolbar>
          <Typography variant="h5" sx={{ fontWeight: 'bold', color: '#4fc3f7' }}>
            DineOS
          </Typography>
        </Toolbar>
        <Divider sx={{ bgcolor: '#333' }} />
        
        <List>
          {menuItems.filter(item => item.roles.includes(role)).map((item) => (
            <ListItem key={item.text} disablePadding>
              <ListItemButton 
                component={NavLink}
                to={item.path}
                sx={{ 
                  '&:hover': { bgcolor: '#283593' },
                  '&.active': {   
                    bgcolor: '#283593', 
                    borderLeft: '4px solid #4fc3f7',
                    color: '#4fc3f7',
                    '& .MuiListItemIcon-root': { color: '#4fc3f7' }
                  },             
                }}
              >
                <ListItemIcon sx={{ color: 'white' }}>
                  {item.icon}
                </ListItemIcon>
                <ListItemText primary={item.text} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </Drawer>

      {/* 3. MAIN CONTENT AREA */}
      <Box
        component="main"
        sx={{ 
          flexGrow: 1, 
          bgcolor: '#f4f6f8', 
          p: 3, 
          minHeight: '100vh',
          mt: '64px'
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
};

export default MainLayout;