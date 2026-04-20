import { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  TextField,
  CircularProgress
} from '@mui/material';

import { orderService } from '../../../api/orderService';
import OrderHistoryTable from './OrderHistoryTable';
import OrderDetailDialog from './OrderDetailDialog';

export default function OrderHistoryPage() {
  const [orders, setOrders] = useState([]);
  const [search, setSearch] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const [loading, setLoading] = useState(false);

  const [open, setOpen] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState(null);

  const fetchHistory = async () => {
    try {
      setLoading(true);
      const res = await orderService.staff.getHistory({ search, fromDate, toDate });
      setOrders(res);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchHistory();
  }, []);

  useEffect(() => {
    const delay = setTimeout(fetchHistory, 400);
    return () => clearTimeout(delay);
  }, [search, fromDate, toDate]);

  const handleView = async (id) => {
    const res = await orderService.staff.getById(id);
    setSelectedOrder(res);
    setOpen(true);
  };

  return (
    <Box p={3}>
      <Typography variant="h5" mb={2}>
        Order History
      </Typography>

      {/* FILTER */}
      <Box display="flex" gap={2} mb={2}>
        <TextField
          size="small"
          placeholder="Search #668f"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <TextField
          size="small"
          type="date"
          value={fromDate}
          onChange={(e) => setFromDate(e.target.value)}
        />

        <TextField
          size="small"
          type="date"
          value={toDate}
          onChange={(e) => setToDate(e.target.value)}
        />
      </Box>

      {/* CONTENT */}
      {loading ? (
        <Box textAlign="center" mt={5}>
          <CircularProgress />
        </Box>
      ) : (
        <OrderHistoryTable orders={orders} onView={handleView} />
      )}

      {/* DIALOG */}
      <OrderDetailDialog
        open={open}
        onClose={() => setOpen(false)}
        order={selectedOrder}
      />
    </Box>
  );
}