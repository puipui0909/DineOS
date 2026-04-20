import {
  Box,
  TextField,
  Button,
  MenuItem,
  Select,
  InputLabel,
  FormControl
} from '@mui/material';
import { useEffect, useState } from 'react';
import { menuService } from '../../../../api/menuService';
import { categoryService } from '../../../../api/categoryService';



const MenuForm = ({ onCreated, initialData }) => {
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
  const [file, setFile] = useState(null);
  const [imageUrl, setImageUrl] = useState('');
  const [categories, setCategories] = useState([]);
  const [categoryId, setCategoryId] = useState('');
  const [newCategory, setNewCategory] = useState('');

  // Set initial values when initialData changes
  useEffect(() => {
    if (initialData) {
      setName(initialData.name);
      setPrice(initialData.price.toString());
      setImageUrl(initialData.imageUrl);
      setCategoryId(initialData.categoryId);
    } else {
      setName('');
      setPrice('');
      setFile(null);
      setImageUrl('');
      setCategoryId('');
    }
  }, [initialData]);

  const handleCreateCategory = async () => {
    if (!newCategory.trim()) return;

    const res = await categoryService.create({
      name: newCategory
    });

    // reload lại category
    await fetchCategories();

    setNewCategory('');
  };
  
  // load category
  useEffect(() => {
    fetchCategories();
  }, []);

  const fetchCategories = async () => {
    const data = await categoryService.getAll();
    setCategories(data);
  };

  // upload ngay khi chọn file
  const handleFileChange = async (e) => {
    const selectedFile = e.target.files[0];
    if (!selectedFile) return;

    setFile(selectedFile);

    const formData = new FormData();
    formData.append("file", selectedFile);

    const res = await menuService.upload(formData);
    setImageUrl(res.url || res.imageUrl);
  };

  const handleSubmit = async () => {
    if (initialData) {
      await menuService.update(initialData.id, {
        name,
        price: Number(price),
        categoryId,
        imageUrl
      });
    } else {
      await menuService.create({
        name,
        price: Number(price),
        categoryId,
        imageUrl
      });
    }
    onCreated();

    setName('');
    setPrice('');
    setFile(null);
    setImageUrl('');
    setCategoryId('');
  };

  return (
    <Box sx={{ mt: 2 }}>
      <TextField
        label="Name"
        fullWidth
        margin="normal"
        value={name}
        onChange={(e) => setName(e.target.value)}
      />

      <TextField
        label="Price"
        type="number"
        fullWidth
        margin="normal"
        value={price}
        onChange={(e) => setPrice(e.target.value)}
      />

      {/* CATEGORY */}
      <FormControl fullWidth margin="normal">
        <InputLabel>Category</InputLabel>
        <Select
          value={categoryId}
          onChange={(e) => setCategoryId(e.target.value)}
        >
          {categories.map((c) => (
            <MenuItem key={c.id} value={c.id}>
              {c.name}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      {/* ADD NEW CATEGORY */}
      <Box sx={{ display: 'flex', gap: 1, mt: 1 }}>
        <TextField
          label="New category"
          size="small"
          fullWidth
          value={newCategory}
          onChange={(e) => setNewCategory(e.target.value)}
        />

        <Button variant="contained" onClick={handleCreateCategory}>
          Add
        </Button>
      </Box>

      {/* IMAGE */}
      <input type="file" onChange={handleFileChange} />

      {imageUrl && (
        <Box mt={2}>
          <img src={imageUrl} alt="preview" width="100%" />
        </Box>
      )}

      <Button
        variant="contained"
        fullWidth
        sx={{ mt: 2 }}
        onClick={handleSubmit}
      >
        {initialData ? 'Update' : 'Create'}
      </Button>
    </Box>
  );
};

export default MenuForm;