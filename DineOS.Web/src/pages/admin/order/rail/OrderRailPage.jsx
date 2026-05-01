import { useEffect, useState, useCallback } from 'react';
import { Box, Grid, CircularProgress } from '@mui/material';
import OrderColumn from './OrderColumn';
import { orderService } from '../../../../api/orderService';
import { startOrderRealtime, stopOrderRealtime } from '../../../../api/orderRealtime';

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
        .filter(o => o && o.isActive)
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
  
  useEffect(() => {
    console.log("🚀 INIT REALTIME");

    startOrderRealtime(
      null, // onNewOrder (bạn chưa dùng)
      (updatedOrder) => {
        console.log("🔥 REALTIME HIT:", updatedOrder);

        setOrders(prev => {
          const index = prev.findIndex(o => o.id === updatedOrder.id);

          if (index === -1) return [updatedOrder, ...prev];

          const updated = [...prev];
          updated[index] = updatedOrder;
          return updated;
        });
      }
    );

  }, []);

  // =========================
  // REALTIME SIGNALR
  // =========================
  const handleRealtimeUpdate = useCallback((updatedOrder) => {
    console.log("🔥 RAIL PAGE UPDATE:", updatedOrder.id);
    setOrders(prev => {
      const normalized = {
        ...updatedOrder,
        status: updatedOrder.status?.toString().toUpperCase().replace(/\s/g, ''),
        items: updatedOrder.items ?? [],
        table: updatedOrder.table ?? {},
      };
      const index = prev.findIndex(o => o.id === normalized.id);
      if (index === -1) return [normalized, ...prev];
      const updated = [...prev];
      updated[index] = normalized;
      return updated;
    });
  }, []);
  useEffect(() => {
    // Chúng ta chỉ cần nghe UpdateOrder ở trang này, NewOrder đã có Layout lo hoặc fetch đầu trang lo
    startOrderRealtime(null, handleRealtimeUpdate);

    return () => {
      console.log("Cleanup Rail Page Realtime");
      stopOrderRealtime(null, handleRealtimeUpdate);
    };
  }, [handleRealtimeUpdate]);

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
  const openOrders = orders;
  const inProgressOrders = orders;
  const closedOrders = orders;

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