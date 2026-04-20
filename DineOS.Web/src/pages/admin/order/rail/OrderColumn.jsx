import { Box, Button } from '@mui/material';
import OrderCard from './OrderCard';

export default function OrderColumn({ title, status, orders, onReload }) {
  const getOldestItemTime = (order) => {
    const pending = order.items?.filter(i => !i.isSentToKitchen) || [];

    if (pending.length > 0) {
      return Math.min(...pending.map(i => new Date(i.createdAt).getTime()));
    }

    const all = order.items ?? [];
    if (all.length === 0) return Infinity;

    return Math.min(...all.map(i => new Date(i.createdAt).getTime()));
  };
  const hasPending = (order) =>
    order.items?.some(i => !i.isSentToKitchen);

  const filtered = (orders ?? [])
    .filter(o => {
      if (!o) return false;

      if (status === 'OPEN') {
        return hasPending(o); // còn món chưa gửi → luôn ở OPEN
      }

      if (status === 'INPROGRESS') {
        return !hasPending(o) && o.status !== 'CLOSED'; // đã gửi hết
      }

      if (status === 'CLOSED') {
        return o.status === 'CLOSED';
      }

      return false;
    })
    .sort((a, b) => getOldestItemTime(a) - getOldestItemTime(b));
  return (
    <Box>
      {/* HEADER */}
      <Box
        sx={{
          mb: 2,
          py: 1,
          borderRadius: 5,
          backgroundColor: '#e0e0e0',
          textAlign: 'center',
          fontWeight: 600
        }}
      >
        {title}
      </Box>

      {/* LIST */}
      <Box display="flex" flexDirection="column" gap={2}>
        {filtered.map(order => {
        return (
          <OrderCard
            key={order.id}
            order={order}
            onReload={onReload}
          />
        );
      })}
      </Box>
    </Box>
  );
}