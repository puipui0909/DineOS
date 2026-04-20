import { Box, Typography, Button } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';

const TableHeader = ({ onAddClick, isAdmin }) => {
  return (
    <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
      <Box>
        <Typography variant="h5" fontWeight="bold">
          Table Management
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Manage your restaurant tables and availability
        </Typography>
      </Box>
      {isAdmin && (
        <Button variant="contained" startIcon={<AddIcon />} sx={{ borderRadius: '20px' }} onClick={onAddClick}>
          Add New Table
        </Button>
      )}
    </Box>
  );
};

export default TableHeader;