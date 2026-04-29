import { useEffect, useState, useCallback } from 'react';
import { Box, Grid, CircularProgress } from '@mui/material';
import OrderColumn from './OrderColumn';
import { orderService } from '../../../../api/orderService';
import { startOrderRealtime } from '../../../../api/orderRealtime';

export default function OrderRailPage() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);

  // =========================
  // FETCH INITIAL DATA
  // =========================
  const fetchOrders = useCallback(async () => {
    try {
      setLoading(true);

      let data = [];

      if (orderService.staff.getAll) {
        data = await orderService.staff.getAll();
      } else if (orderService.staff.getOrders) {
        data = await orderService.staff.getOrders();
      }

      const safe = (data ?? [])
        .filter(o => o)
        .map(o => ({
          ...o,
          status: o.status?.toUpperCase(),
          items: o.items ?? [],
          table: o.table ?? {},
        }));

      setOrders(safe);
    } catch (err) {
      console.error('Fetch orders error:', err);
      setOrders([]);
    } finally {
      setLoading(false);
    }
  }, []);

  // =========================
  // LOAD LẦN ĐẦU
  // =========================
  useEffect(() => {
    fetchOrders();
  }, [fetchOrders]);

  // =========================
  // REALTIME SIGNALR
  // =========================
  useEffect(() => {
    startOrderRealtime(

      // 🟢 NEW ORDER
      (newOrder) => {
        setOrders(prev => {
          if (!newOrder) return prev;

          const normalized = {
            ...newOrder,
            status: newOrder.status?.toUpperCase(),
            items: newOrder.items ?? [],
            table: newOrder.table ?? {},
          };

          return [normalized, ...prev];
        });
      },

      // 🟡 UPDATE ORDER
      (updatedOrder) => {
        setOrders(prev => {
          if (!updatedOrder) return prev;

          const normalized = {
            ...updatedOrder,
            status: updatedOrder.status?.toUpperCase(),
            items: updatedOrder.items ?? [],
            table: updatedOrder.table ?? {},
          };

          const exists = prev.some(o => o.id === normalized.id);

          if (!exists) {
            return [normalized, ...prev];
          }

          return prev.map(o =>
            o.id === normalized.id ? normalized : o
          );
        });
      }
    );
  }, []);

  // =========================
  // LOADING UI
  // =========================
  if (loading && orders.length === 0) {
    return (
      <Box display="flex" justifyContent="center" mt={5}>
        <CircularProgress />
      </Box>
    );
  }

  // =========================
  // FILTER ĐÚNG LOGIC
  // =========================
  const openOrders = orders.filter(o => o.status === "OPEN");
  const inProgressOrders = orders.filter(o => o.status === "INPROGRESS");
  const closedOrders = orders.filter(o => o.status === "CLOSED");

  return (
    <Box p={2}>
      <Grid container spacing={2}>

        {/* OPEN */}
        <Grid size={4}>
          <OrderColumn
            title="OPEN"
            status="OPEN"
            orders={openOrders}
            onReload={fetchOrders}
          />
        </Grid>

        {/* IN_PROGRESS */}
        <Grid size={4}>
          <OrderColumn
            title="IN PROGRESS"
            status="INPROGRESS"
            orders={inProgressOrders}
            onReload={fetchOrders}
          />
        </Grid>

        {/* CLOSED */}
        <Grid size={4}>
          <OrderColumn
            title="CLOSED"
            status="CLOSED"
            orders={closedOrders}
            onReload={fetchOrders}
          />
        </Grid>

      </Grid>
    </Box>
  );
}