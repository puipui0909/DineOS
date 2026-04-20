import { Box, TextField, Chip } from '@mui/material';

const MenuFilter = ({ 
  categories, 
  searchText, 
  onSearchChange, 
  selectedCategory, 
  onCategoryChange 
}) => {
  return (
    <Box sx={{
      display: 'flex',
      alignItems: 'center',
      gap: 2,
      mb: 3,
      bgcolor: '#fff',
      p: 2,
      borderRadius: 3,
      boxShadow: 1
    }}>
      <TextField
        placeholder="Search..."
        size="small"
        sx={{ flex: 1 }}
        value={searchText}
        onChange={(e) => onSearchChange(e.target.value)}
      />

      {categories.map((cat) => (
        <Chip
          key={cat.id}
          label={cat.name}
          clickable
          onClick={() => onCategoryChange(cat.id)}
          color={selectedCategory === cat.id ? 'primary' : 'default'}
        />
      ))}
    </Box>
  );
};

export default MenuFilter;