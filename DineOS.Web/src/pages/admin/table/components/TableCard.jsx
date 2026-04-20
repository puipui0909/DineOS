import {
  Card, CardContent, Typography,
  Box, Chip, Button
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import DeleteIcon from '@mui/icons-material/Delete';
import { IconButton, Switch, FormControlLabel } from '@mui/material';
const getStatusColor = (status) => {
  switch (status) {
    case 'Available': return 'success';
    case 'Occupied': return 'warning';
    case 'Reserved': return 'info';
    case 'OutOfService': return 'error';
    default: return 'default';
  }
};

const TableCard = ({ table, onDelete, onReserve, onCancelReserve, onToggleOutOfService }) => {
  const navigate = useNavigate();
  return (
    <Card sx={{ borderRadius: 3, boxShadow: 2, p: 2, position: 'relative' }}>

      {/* Header */}
      <Box display="flex" justifyContent="space-between">
        <Typography fontWeight="bold">{table.name}</Typography>

        <Box>
          <Chip
            label={table.status}
            size="small"
            color={getStatusColor(table.status)}
          />

          <IconButton
            size="small"
            color="error"
            onClick={() => onDelete(table.id)}
          >
            <DeleteIcon fontSize="small" />
          </IconButton>
        </Box>
        <FormControlLabel
          control={
            <Switch
              checked={table.status === 'OutOfService'}
              onChange={(e) => {
                onToggleOutOfService(table.id, e.target.checked);
              }}
              color="error"
            />
          }
        />
      </Box>

      {/* Body */}
      <CardContent sx={{ px: 0 }}>
        {table.status === 'Available' && (
          <Box display="flex" gap={1}>
            <Button
              fullWidth
              variant="contained"
              onClick={() => navigate(`/admin/order/${table.id}`)}
            >
              Order
            </Button>
            <Button fullWidth variant="contained" onClick={() => onReserve(table.id)}>
              Reserve
            </Button>
          </Box>
        )}
        {table.status === 'Reserved' && (
          <Box display="flex" gap={1}>
            <Button fullWidth variant="contained">
              Order
            </Button>
            <Button
              fullWidth
              color="warning"
              onClick={() => onCancelReserve(table.id, 'Available')}
            >
              Cancel Reserve
            </Button>
          </Box>
        )}

        {table.status === 'Occupied' && (
          <Button
            fullWidth
            variant="contained"
            color="warning"
            onClick={() => navigate(`/admin/order/${table.id}`)}
          >
            Continue Order
          </Button>
        )}

        {table.status === 'OutOfService' && (
          <Typography variant="body2" color="error">
            Out of service
          </Typography>
        )}
      </CardContent>
    </Card>
  );
};

export default TableCard;