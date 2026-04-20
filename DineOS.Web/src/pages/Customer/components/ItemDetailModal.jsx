import {
  Dialog,
  DialogContent,
  Typography,
  Button,
  Box,
  TextField
} from '@mui/material';
import { useState, useEffect } from 'react';

export default function ItemDetailModal({ open, item, onClose, onConfirm }) {
  const [quantity, setQuantity] = useState(1);
  const handleConfirm = () => {
    onConfirm(item.id, quantity);
  };

  useEffect(() => {
    if (open) {
      setQuantity(1);
    }
  }, [open]);

  if (!item) return null;

  return (
    <Dialog open={open} onClose={onClose} fullWidth>
      <DialogContent>
        <Typography variant="h6">{item.name}</Typography>
        <Typography mb={2}>{item.price.toLocaleString()}đ</Typography>

        <Box display="flex" alignItems="center" gap={2} mb={2}>
          <Button onClick={() => setQuantity((q) => Math.max(1, q - 1))}>-</Button>
          <Typography>{quantity}</Typography>
          <Button onClick={() => setQuantity((q) => q + 1)}>+</Button>
        </Box>

        <TextField fullWidth placeholder="Ghi chú..." />

        <Box mt={2}>
          <Button
            fullWidth
            variant="contained"
            onClick={handleConfirm}
          >
            Xác nhận
          </Button>
        </Box>
      </DialogContent>
    </Dialog>
  );
}