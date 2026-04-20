import React, { useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Grid,
  Typography,
  Button,
  Card,
  CardContent,
  Divider,
  Tabs,
  Tab
} from '@mui/material';

import { orderService } from '../../../api/orderService';
import { useParams } from 'react-router-dom';
import { useMenuPage } from '../../../hooks/useMenuPage';
import MenuCard from '../menu/components/MenuCard';
import MenuFilter from '../menu/components/MenuFilter';
import { paymentService } from '../../../api/paymentService';
import { useMenuFilter } from '../../../hooks/useMenuFilter';

export default function OrderPage() {
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const { tableId } = useParams();
  const [cart, setCart] = useState([]);
  const [tab, setTab] = useState(0);
  const [searchText, setSearchText] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('All');
  const { fetchMenu, menuItems, categories } = useMenuPage();
  const [paymentMethod, setPaymentMethod] = useState('Cash');
  const [showPayment, setShowPayment] = useState(false);
  const [paying, setPaying] = useState(false);
  const normalizedStatus = (order?.status || '').toUpperCase();
  const filteredMenu = useMenuFilter(
    menuItems,
    searchText,
    selectedCategory
  );


  const navigate = useNavigate();
  const hasFetched = useRef(false);

  useEffect(() => {
    if (hasFetched.current) return;
    hasFetched.current = true;
    const init = async () => {
      await fetchOrder();
      await fetchMenu();
    };
    init();
  }, [tableId]);

  const fetchOrder = async () => {
    try {
      setLoading(true);
      const data = await orderService.staff.getByTable(tableId);
      setOrder(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };
  // ================= ACTION =================

  const handleAddItems = async () => {
    try {
      for (const item of cart) {
        await orderService.staff.addItem(
          order.id,
          item.menuItemId,
          item.quantity,
          item.createdAt,
        );
      }

      setCart([]);
      fetchOrder();
    } catch (err) {
      console.error(err);
    }
  };

  const handleAddToCart = (item) => {
    setCart((prev) => {
      const exist = prev.find(i => i.menuItemId === item.id);

      if (exist) {
        return prev.map(i =>
          i.menuItemId === item.id
            ? { ...i, quantity: i.quantity + 1 }
            : i
        );
      }

      return [
        ...prev,
        {
          menuItemId: item.id,
          name: item.name,
          price: item.price,
          quantity: 1
        }
      ];
    });
  };

  // giảm số lượng
  const handleDecrease = (menuItemId) => {
    setCart(prev =>
      prev
        .map(item =>
          item.menuItemId === menuItemId
            ? { ...item, quantity: item.quantity - 1 }
            : item
        )
        .filter(item => item.quantity > 0)
    );
  };

// xoá hẳn món
  const handleRemoveItem = (menuItemId) => {
    setCart(prev =>
      prev.filter(item => item.menuItemId !== menuItemId)
    );
  };
  if (loading) return <Typography>Loading...</Typography>;
  if (!order) return <Typography>Loading...</Typography>;

  const handlePayment = async () => {
  try {
    setPaying(true);

    await paymentService.pay(order.id, paymentMethod);

    setOrder(null);
    setCart([]);
    setShowPayment(false);
    setPaymentMethod('Cash'); // reset

    navigate('/admin/table');
  } catch (err) {
    console.error(err);
  } finally {
    setPaying(false);
  }
};

  
  const orderItems = order?.items ?? [];

  const total =
    orderItems.reduce((s, i) => s + i.price * i.quantity, 0) +
    cart.reduce((s, i) => s + i.price * i.quantity, 0);

  return (
    <Box p={3}>
      <Typography variant="h5" mb={2}>
        {order.table?.name}
      </Typography>

      <Grid container spacing={2}>

        {/* ===== LEFT MENU ===== */}
        <Grid size={9}>
          <MenuFilter
            categories={categories}
            searchText={searchText}
            onSearchChange={setSearchText}
            selectedCategory={selectedCategory}
            onCategoryChange={setSelectedCategory}
          />
          <Grid container spacing={2}>
            {filteredMenu?.map((item) => (
              <Grid size={4} key={item.id}>
                <MenuCard
                  item={item}
                  mode="order"
                  onClick={handleAddToCart}
                />
              </Grid>
            ))}
          </Grid>
        </Grid>
        {/* ===== RIGHT PANEL ===== */}
        <Grid size={3}>
          <Card>
            <CardContent>

              {/* TABS */}
              <Tabs value={tab} onChange={(e, v) => setTab(v)}>
                <Tab label="Gọi món" />
                <Tab label="Đơn" />
              </Tabs>

              <Divider sx={{ my: 2 }} />

              {/* ===== TAB 1: CART ===== */}
              {tab === 0 && (
                <>
                  {cart.length === 0 && (
                    <Typography>Chưa có món</Typography>
                  )}

                  {cart.map((item) => (
                    <Box
                      key={item.menuItemId}
                      display="flex"
                      justifyContent="space-between"
                      alignItems="center"
                      mb={1}
                    >
                      <Box>
                        <Typography>
                          x{item.quantity} {item.name}
                        </Typography>
                        <Typography fontSize={13}>
                          {item.price * item.quantity}đ
                        </Typography>
                      </Box>

                      <Box>
                        <Button
                          size="medium"
                          onClick={() => handleDecrease(item.menuItemId)}
                        >
                          -
                        </Button>

                        <Button
                          size="medium"
                          color="error"
                          onClick={() => handleRemoveItem(item.menuItemId)}
                        >
                          X
                        </Button>
                      </Box>
                    </Box>
                  ))}

                  <Button
                    fullWidth
                    variant="contained"
                    sx={{ mt: 2 }}
                    onClick={handleAddItems}
                    disabled={
                      cart.length === 0 ||
                      (normalizedStatus !== 'OPEN' && normalizedStatus !== 'INPROGRESS')
                    }
                  >
                    Thêm món
                  </Button>
                </>
              )}

              {/* ===== TAB 2: SENT ITEMS ===== */}
              {tab === 1 && (
                <>
                  {orderItems.length === 0 && (
                    <Typography>Chưa có món</Typography>
                  )}

                  {orderItems.map((item) => (
                    <Box key={item.id} mb={1}>
                      <Typography>
                        x{item.quantity} {item.name}
                      </Typography>
                      <Typography fontSize={13}>
                        {item.price * item.quantity}đ
                      </Typography>
                    </Box>
                  ))}
                </>
              )}

              {/* ===== FOOTER ===== */}
              <Divider sx={{ my: 2 }} />

              <Typography fontWeight="bold">
                Tổng: {total}đ
              </Typography>

              {normalizedStatus === 'CLOSED' && (
                <Box mt={2}>
                  {!showPayment && (
                    <Button
                      fullWidth
                      variant="contained"
                      color="success"
                      onClick={() => setShowPayment(true)}
                    >
                      Thanh toán
                    </Button>
                  )}

                  {showPayment && (
                    <>
                      <Typography mt={2}>Chọn phương thức:</Typography>

                      <Box display="flex" gap={1} mt={1}>
                        {['Cash', 'Transfer'].map(m => (
                          <Button
                            key={m}
                            variant={paymentMethod === m ? 'contained' : 'outlined'}
                            onClick={() => setPaymentMethod(m)}
                          >
                            {m}
                          </Button>
                        ))}
                      </Box>

                      <Button
                        fullWidth
                        variant="contained"
                        color="success"
                        sx={{ mt: 2 }}
                        onClick={handlePayment}
                        disabled={paying}
                      >
                        {paying ? 'Đang xử lý...' : 'Xác nhận thanh toán'}
                      </Button>
                    </>
                  )}
                </Box>
              )}

              <Button
                fullWidth
                variant="contained"
                sx={{ mt: 2 }}
                disabled={
                  normalizedStatus !== 'INPROGRESS'
                }
                onClick={async () => {
                  if (normalizedStatus === 'INPROGRESS') {
                    await orderService.staff.closeOrder(order.id);
                    await fetchOrder();
                  } 
                }}
              >
                Đóng đơn
                
              </Button>

            </CardContent>
          </Card>
        </Grid>

      </Grid>
    </Box>
  );
}