import {
  Drawer,
  Box,
  Typography,
  Button,
  Divider
} from '@mui/material';

export default function OrderSummary({ open, order, onClose, onSend }) {
  const items = order?.orderItems || [];

  const total = items.reduce(
    (sum, i) => sum + i.price * i.quantity,
    0
  );

  return (
    <Drawer anchor="bottom" open={open} onClose={onClose}>
      <Box p={2}>
        <Typography variant="h6">Thông tin đơn</Typography>

        {items.length === 0 && (
          <Typography mt={2}>Chưa có món nào</Typography>
        )}

        {items.map((item, index) => (
          <Box key={index} display="flex" justifyContent="space-between" mt={1}>
            <Typography>
              {item.name} x{item.quantity}
            </Typography>

            <Typography>
              {(item.price * item.quantity).toLocaleString()}đ
            </Typography>
          </Box>
        ))}

        <Divider sx={{ my: 2 }} />

        <Typography fontWeight="bold">
          Tổng cộng: {total.toLocaleString()}đ
        </Typography>

        <Box mt={2} display="flex" gap={1}>
          <Button fullWidth variant="outlined" onClick={onClose}>
            Đóng
          </Button>

          <Button
            fullWidth
            variant="contained"
            onClick={onSend}
            disabled={items.length === 0}
          >
            Xác nhận
          </Button>
        </Box>
      </Box>
    </Drawer>
  );
}