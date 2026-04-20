import { Box, Typography, Button } from '@mui/material';
import { orderService } from "../../../../api/orderService";

export default function OrderCard({ order, onReload }) {
  if (!order) return null;

  const allItems = order.items ?? [];

  const pendingItems = allItems
    .filter(i => !i.isSentToKitchen)
    .sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));

  const sentItems = allItems
    .filter(i => i.isSentToKitchen)
    .sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
  
  const hasPending = pendingItems.length > 0;

  return (
    <Box
      sx={{
        backgroundColor: '#d9d9d9',
        borderRadius: 3,
        p: 2
      }}
    >
      {/* ORDER ID */}
      <Typography fontSize={12} color="gray">
        #{order.id?.slice(0, 4)}
      </Typography>

      {/* TABLE NAME */}
      <Typography fontWeight="bold" mb={1}>
        {order.table?.name}
      </Typography>

      {/* ITEMS */}
      <Box display="flex" flexDirection="column" gap={1} mb={2}>
        {pendingItems.map(item => (
          <Box
            key={item.id}
            sx={{
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              px: 1,
              py: 0.5,
              borderRadius: 1,
              backgroundColor: item.isSentToKitchen ? '#eee' : '#fff',
            }}
          >
            <Typography fontSize={14}>
              x{item.quantity} {item.name}
            </Typography>

            <Typography fontSize={12} color="green">
              NEW
            </Typography>
          </Box>
        ))}

        {sentItems.map(item => (
          <Box
            key={item.id}
            sx={{
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              px: 1,
              py: 0.5,
              borderRadius: 1,
              backgroundColor: '#eee',
              opacity: 0.5
            }}
          >
            <Typography fontSize={14}>
              x{item.quantity} {item.name}
            </Typography>
          </Box>
        ))}
      </Box>

      {/* ACTION */}
      {hasPending && (
        <Button
          fullWidth
          variant="contained"
          disabled={!hasPending}
          onClick={async () => {
            await orderService.staff.sendToKitchen(order.id);
            onReload();
          }}
        >
          Start Progress
        </Button>
      )}

      {order.status === 'CLOSED' && (
        <Button fullWidth disabled>
          Completed
        </Button>
      )}
    </Box>
  );
}