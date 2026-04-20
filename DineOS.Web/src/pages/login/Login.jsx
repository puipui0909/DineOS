import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, Container, TextField, Button, Typography, Paper } from '@mui/material';
import { useAuth } from '../../hooks/useAuth';
import { useNotification } from '../../hooks/useNotification';
import styles from './Login.module.css';

const Login = () => {
  const navigate = useNavigate();
  const { login, loading } = useAuth();
  const { showSuccess, showError } = useNotification();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();
    console.log("FORM SUBMIT");

    if (!username || !password) {
      showError('Vui lòng nhập đầy đủ thông tin!');
      return;
    }

    try {
      const user = await login(username, password);
      console.log("LOGIN SUCCESS");
      showSuccess('Đăng nhập thành công!');

      // Redirect dựa trên role
      navigate('/admin/dashboard');
    } catch (err) {
      showError('Sai tài khoản hoặc mật khẩu!');
    }
  };

  return (
    <Box className={styles.loginContainer}>
      <Container maxWidth="lg" sx={{ height: '100vh', display: 'flex', alignItems: 'center' }}>
        <Box sx={{ display: 'flex', width: '100%', gap: 4 }}>
          {/* Left Side - Image */}
          <Box sx={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <Paper
              elevation={0}
              sx={{
                width: '100%',
                aspectRatio: '1',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                backgroundColor: '#f5f5f5',
                borderRadius: 2,
              }}
            >
              <Box
                sx={{
                  width: '80%',
                  height: '80%',
                  border: '2px solid #ddd',
                  borderRadius: 2,
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  flexDirection: 'column',
                  color: '#999',
                }}
              >
                {/* Logo */}
                <Typography variant="h1" sx={{ fontSize: '80px', color: '#e0e0e0' }}>
                  🍽️
                </Typography>
                <Typography variant="h6" sx={{ color: '#bbb', marginTop: 2 }}>
                  DineOS
                </Typography>
              </Box>
            </Paper>
          </Box>

          {/* Right Side - Form */}
          <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
            <Typography variant="h4" sx={{ marginBottom: 4, fontWeight: 'bold' }}>
              Đăng nhập
            </Typography>

            <form onSubmit={handleLogin}>
              {/* Tài khoản */}
              <TextField
                fullWidth
                label="Tài khoản"
                placeholder="Nhập tài khoản"
                variant="outlined"
                margin="normal"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                disabled={loading}
                sx={{
                  marginBottom: 2,
                  '& .MuiOutlinedInput-root': {
                    backgroundColor: '#f9f9f9',
                  },
                }}
              />

              {/* Mật khẩu */}
              <TextField
                fullWidth
                label="Mật khẩu"
                placeholder="Nhập mật khẩu"
                type="password"
                variant="outlined"
                margin="normal"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                disabled={loading}
                sx={{
                  marginBottom: 3,
                  '& .MuiOutlinedInput-root': {
                    backgroundColor: '#f9f9f9',
                  },
                }}
              />

              {/* Đăng nhập Button */}
              <Button
                type="submit"
                fullWidth
                variant="contained"
                size="large"
                disabled={loading}
                sx={{
                  backgroundColor: '#1976d2',
                  color: 'white',
                  padding: '12px',
                  fontSize: '16px',
                  fontWeight: 'bold',
                  textTransform: 'none',
                  '&:hover': {
                    backgroundColor: '#1565c0',
                  },
                }}
              >
                {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
              </Button>
            </form>

            {/* Demo text */}
            <Typography
              variant="caption"
              sx={{
                marginTop: 2,
                color: '#999',
                textAlign: 'center',
              }}
            >
              Demo: admin / admin123
            </Typography>
          </Box>
        </Box>
      </Container>
    </Box>
  );
};

export default Login;