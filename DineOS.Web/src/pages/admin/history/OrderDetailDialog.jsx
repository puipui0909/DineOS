import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box
} from '@mui/material';

export default function OrderDetailDialog({ open, onClose, order }) {

  const formatDateTime = (date) => {
    if (!date) return 'N/A';
    return new Date(date).toLocaleString('vi-VN');
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Order Detail</DialogTitle>

      <DialogContent>
        {order && (
          <>
            <Typography>
              <b>Table:</b> {order?.table?.name}
            </Typography>

            <Typography>
              <b>Status:</b> {order?.status}
            </Typography>

            <Typography>
              <b>Created:</b> {formatDateTime(order?.createdAt)}
            </Typography>

            <Typography>
              <b>Payment:</b> {order?.paymentMethod || 'N/A'}
            </Typography>

            <Box mt={2}>
              <Typography><b>Items:</b></Typography>

              {order?.items?.map((item) => (
                <Box
                  key={item.id}
                  display="flex"
                  justifyContent="space-between"
                  mt={1}
                >
                  <Box>
                    <div>{item.name} x{item.quantity}</div>
                    <Typography variant="caption" color="gray">
                      {formatDateTime(item.createdAt)}
                    </Typography>
                  </Box>

                  <span>{item.price}k</span>
                </Box>
              ))}
            </Box>

            <Box mt={2}>
              <Typography>
                <b>Total:</b> {order?.total}k
              </Typography>
            </Box>
          </>
        )}
      </DialogContent>

      <DialogActions>
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </Dialog>
  );
}