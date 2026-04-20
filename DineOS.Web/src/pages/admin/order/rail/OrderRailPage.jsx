import { useEffect, useState, useCallback } from 'react';
import { Box, Grid, CircularProgress } from '@mui/material';
import OrderColumn from './OrderColumn';
import { orderService } from '../../../../api/orderService';

export default function OrderRailPage() {
  const [orders, setOrders] = useState([]); 
  const [loading, setLoading] = useState(false);

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

  useEffect(() => {
    fetchOrders();
  }, [fetchOrders]);

  if (loading && orders.length === 0) {
    return (
      <Box display="flex" justifyContent="center" mt={5}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box p={2}>
      <Grid container spacing={2}>

        {/* OPEN */}
        <Grid size={4}>
          <OrderColumn
            title="OPEN"
            status="OPEN"
            orders={orders.filter(o => o.isActive)}
            onReload={fetchOrders}
          />
        </Grid>

        {/* IN_PROGRESS */}
        <Grid size={4}>
          <OrderColumn
            title="IN PROGRESS"
            status="INPROGRESS"
            orders={orders.filter(o => o.isActive)}
            onReload={fetchOrders}
          />
        </Grid>

        {/* CLOSED */}
        <Grid size={4}>
          <OrderColumn
            title="CLOSED"
            status="CLOSED"
            orders={orders}
            onReload={fetchOrders}
          />
        </Grid>

      </Grid>
    </Box>
  );
}