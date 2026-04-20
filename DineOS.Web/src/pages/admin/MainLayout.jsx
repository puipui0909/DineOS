import React, { useState } from 'react';
import { Menu, MenuItem } from '@mui/material';
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { NavLink, useLocation, Outlet } from 'react-router-dom';
import { 
  Box, Drawer, AppBar, Toolbar, List, Typography, 
  Divider, ListItem, ListItemButton, ListItemIcon, ListItemText, IconButton, Avatar 
} from '@mui/material';
import { 
  Dashboard as DashboardIcon, 
  ShoppingCart as OrderIcon, 
  RestaurantMenu as MenuIcon, 
  History as HistoryIcon, 
  Settings as SettingsIcon,
  Notifications as NotificationsIcon,
  TableRestaurant as TableIcon
} from '@mui/icons-material';

const drawerWidth = 240;

const MainLayout = () => {
  const { role, user, logout } = useAuth();
  const navigate = useNavigate();
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
  
  useEffect(() => {
    if (!role) {
      navigate('/login');
    }
  }, [role]);

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
            <IconButton><NotificationsIcon /></IconButton>

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
            bgcolor: '#1a2035', // Màu tối cho chuyên nghiệp
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
        
        <Divider sx={{ bgcolor: '#333', mt: 'auto' }} />
        <List>
          <ListItem disablePadding>
            <ListItemButton>
              <ListItemIcon sx={{ color: 'white' }}><SettingsIcon /></ListItemIcon>
              <ListItemText primary="Settings" />
            </ListItemButton>
          </ListItem>
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