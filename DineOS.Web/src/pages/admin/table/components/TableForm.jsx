import {
  Dialog, DialogTitle, DialogContent,
  TextField, DialogActions, Button
} from '@mui/material';
import { useState } from 'react';
import { tableService } from '../../../../api/tableService';
import { useAuth } from '../../../../hooks/useAuth';


const TableForm = ({ open, onClose, onCreated }) => {
  const [tableNumber, setTableNumber] = useState('');
  const { user } = useAuth();
  const [quantity, setQuantity] = useState('');
  const handleSubmit = async () => {
    console.log("USER:", user);
    if (!quantity || isNaN(quantity)) return;

    try {
      await tableService.createMultiple({
        quantity: parseInt(quantity),
        restaurantId: user.restaurantId
      });

      setQuantity('');
      onCreated();
      onClose();
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>Add Table</DialogTitle>

      <DialogContent>
        <TextField
          label="Number of Tables"
          fullWidth
          value={quantity}
          onChange={(e) => setQuantity(e.target.value)}
        />
      </DialogContent>

      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit}>
          Create
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default TableForm;