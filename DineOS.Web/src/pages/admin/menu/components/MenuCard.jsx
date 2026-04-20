import {
  Card, CardContent, CardActions,
  Typography, IconButton, Box
} from '@mui/material';

import { menuService } from '../../../../api/menuService';

import { Switch } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';

const MenuCard = ({ item, onEdit, onClick, onDelete, onToggle, mode = 'admin'}) => {
  const isAdminMode = mode === 'admin';
  const isOrderMode = mode === 'order';
  return (
    <Card
      sx={{
        borderRadius: 3,
        boxShadow: 2,
        cursor: isOrderMode && item.isAvailable ? 'pointer' : 'default',
        opacity: item.isAvailable ? 1 : 0.5,
        position: 'relative',
        '&:hover': isOrderMode && item.isAvailable ? { boxShadow: 4 } :  {}
      }}
      onClick={() => isOrderMode && item.isAvailable && onClick?.(item)}
    >
    {!item.isAvailable && (
    <Box
      sx={{
        position: 'absolute',
        top: 0,
        left: 0,
        width: '100%',
        height: '100%',
        bgcolor: 'rgba(0,0,0,0.5)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        zIndex: 1,
        borderRadius: 3,
        pointerEvents: 'none'
      }}
    >
      <Typography
        variant="h6"
        sx={{
          color: '#fff',
          fontWeight: 'bold',
          textTransform: 'uppercase'
        }}
      >
        Unavailable
      </Typography>
    </Box>
    )}
      <Box
        sx={{
          height: 140,
          bgcolor: '#e0e0e0',
          backgroundImage: `url(${import.meta.env.VITE_IMAGE_BASE_URL}${item.imageUrl || ''})`,
          backgroundSize: 'cover',
          backgroundPosition: 'center'
        }}
      />
      <CardContent>
        <Typography fontWeight="bold">{item.name}</Typography>
        <Typography color="text.secondary">{item.price} VND</Typography>
        {isAdminMode && (
          <Box
            mt={1}
            sx={{
              position: 'relative',
              zIndex: 2
            }}
          >
            <Switch
              checked={item.isAvailable}
              onClick={(e) => e.stopPropagation()}
              onChange={() => onToggle?.(item.id)}
            />
          </Box>
        )}
      </CardContent>

       {isAdminMode && (
        <CardActions sx={{ justifyContent: 'flex-end' }}>
          <IconButton size="small" onClick={() => onEdit(item)}>
            <EditIcon fontSize="small" />
          </IconButton>

          <IconButton
            size="small"
            color="error"
            onClick={() => onDelete(item.id)}
          >
            <DeleteIcon fontSize="small" />
          </IconButton>
        </CardActions>
      )}

    </Card>
  );
};

export default MenuCard;