import { Box, Typography, Button } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';

const MenuHeader = ({ onAddClick, isAdmin, isEmptyCategory, onDeleteCategoryClick }) => {
  return (
    <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
      <Box>
        <Typography variant="h5" fontWeight="bold">
          Menu Management
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Manage your restaurant menu items and availability
        </Typography>
      </Box>
      {isAdmin && isEmptyCategory && (
        <Button
          variant="outlined"
          color="error"
          size="small"
          onClick={onDeleteCategoryClick}
        >
          Xoá category này
        </Button>
      )}
      {isAdmin && (
        <Button variant="contained" startIcon={<AddIcon />} sx={{ borderRadius: '20px' }} onClick={onAddClick}>
          Add New dish
        </Button>
      )}
    </Box>
  );
};

export default MenuHeader;