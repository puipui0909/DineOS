import {
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  IconButton,
  Chip
} from '@mui/material';
import VisibilityIcon from '@mui/icons-material/Visibility';

export default function OrderHistoryTable({ orders, onView }) {

  const formatDate = (date) => {
    return new Date(date).toLocaleString();
  };

  return (
    <Table>
      <TableHead>
        <TableRow>
          <TableCell>Order No.</TableCell>
          <TableCell>Table</TableCell>
          <TableCell>Date & Time</TableCell>
          <TableCell>Items</TableCell>
          <TableCell>Status</TableCell>
          <TableCell>Payment</TableCell>
          <TableCell>Total</TableCell>
          <TableCell></TableCell>
        </TableRow>
      </TableHead>

      <TableBody>
        {orders.map((o) => (
          <TableRow key={o.id}>
            <TableCell>{o.orderNo}</TableCell>
            <TableCell>{o.tableName}</TableCell>
            <TableCell>{formatDate(o.createdAt)}</TableCell>
            <TableCell>{o.items}</TableCell>

            <TableCell>
              <Chip label={o.status} color="success" size="small" />
            </TableCell>

            <TableCell>{o.paymentMethod}</TableCell>
            <TableCell>{o.totalAmount}k</TableCell>

            <TableCell>
              <IconButton onClick={() => onView(o.id)}>
                <VisibilityIcon />
              </IconButton>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
}