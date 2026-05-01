import { Drawer, Box, Typography, Button, Divider, IconButton } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
export default function OrderSummary({ open, order, cartItems = [], onClose, onSend, onRemoveItem }) {
  // 1. Lấy món đã được xác nhận (sent) từ API (Clean Architecture: order.items)
  const confirmedItems = order?.items || [];
  
  // 2. Lấy món đang nằm trong giỏ hàng (chưa gửi bếp)
  const pendingItems = cartItems || [];

  // 3. Tính tổng cộng cả 2 nguồn
  const total =
    confirmedItems.reduce((s, i) => s + (i.price * i.quantity || 0), 0) +
    pendingItems.reduce((s, i) => s + (i.price * i.quantity || 0), 0);

  return (
    <Drawer anchor="bottom" open={open} onClose={onClose}>
      <Box p={3} sx={{ maxHeight: '85vh', overflowY: 'auto' }}>
        <Typography variant="h6" fontWeight="bold" mb={2}>Chi tiết đơn hàng</Typography>

        {/* HIỂN THỊ MÓN ĐÃ GỌI (MÀU XANH) */}
        {confirmedItems.length > 0 && (
          <Box mb={2}>
            <Typography variant="subtitle2" color="primary" gutterBottom>Đã gửi bếp:</Typography>
            {confirmedItems.map((item, idx) => (
              <Box key={`confirmed-${idx}`} display="flex" justifyContent="space-between" mb={1}>
                <Typography>x{item.quantity} {item.name}</Typography>
                <Typography>{(item.price * item.quantity).toLocaleString()}đ</Typography>
              </Box>
            ))}
          </Box>
        )}

        {confirmedItems.length > 0 && pendingItems.length > 0 && <Divider sx={{ my: 2, borderStyle: 'dashed' }} />}

        {/* HIỂN THỊ GIỎ HÀNG HIỆN TẠI (MÀU ĐẬM/NGHIÊNG) */}
        {pendingItems.length > 0 && (
          <Box mb={2}>
            <Typography variant="subtitle2" color="secondary" gutterBottom>Món mới chọn:</Typography>
            {pendingItems.map((item, idx) => (
              <Box key={`pending-${idx}`} display="flex" justifyContent="space-between" mb={1}>
                <Typography sx={{ fontStyle: 'italic', fontWeight: 500 }}>
                  x{item.quantity} {item.name}
                </Typography>
                <Typography>{(item.price * item.quantity).toLocaleString()}đ</Typography>
                <IconButton
                  size="small"
                  color="error"
                  onClick={() => onRemoveItem(item.id)}
                >
                  <DeleteIcon fontSize="small" />
                </IconButton>
              </Box>
            ))}
          </Box>
        )}

        {confirmedItems.length === 0 && pendingItems.length === 0 && (
          <Typography py={3} textAlign="center" color="text.secondary">Chưa có món nào</Typography>
        )}

        <Divider sx={{ my: 2 }} />

        <Box display="flex" justifyContent="space-between" mb={3}>
          <Typography variant="h6" fontWeight="bold">Tổng cộng:</Typography>
          <Typography variant="h6" color="error" fontWeight="bold">
            {total.toLocaleString()}đ
          </Typography>
        </Box>

        <Box display="flex" gap={2}>
          <Button fullWidth variant="outlined" onClick={onClose} sx={{ py: 1.5 }}>
            Quay lại
          </Button>
          
          {/* Nút này chỉ sáng khi có món mới để gửi */}
          <Button
            fullWidth
            variant="contained"
            color="success"
            onClick={onSend}
            disabled={pendingItems.length === 0}
            sx={{ py: 1.5, fontWeight: 'bold' }}
          >
            {confirmedItems.length > 0 ? 'Gửi thêm món' : 'Xác nhận đặt'}
          </Button>
        </Box>
      </Box>
    </Drawer>
  );
}