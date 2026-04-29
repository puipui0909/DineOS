import React, { useEffect, useState } from 'react';
import { Box, CircularProgress, FormControl } from '@mui/material';
import { Dialog, DialogTitle, DialogContent, Button, DialogActions,InputLabel, MenuItem, Select } from '@mui/material';
import { useAuth } from '../../../hooks/useAuth';
import { useMenuPage } from '../../../hooks/useMenuPage';
import { useMenuFilter } from '../../../hooks/useMenuFilter';
import { useMenuDialog } from '../../../hooks/useMenuDialog';
import { menuService } from '../../../api/menuService';
import { categoryService } from '../../../api/categoryService';
import DeleteConfirmDialog from './components/DeleteConfirmDialog';

import MenuHeader from './components/MenuHeader';
import MenuFilter from './components/MenuFilter';
import MenuGrid from './components/MenuGrid';
import MenuForm from './components/MenuForm';

const Menu = () => {
  const { role } = useAuth();
  const isAdmin = role === 'Admin';

  const [searchText, setSearchText] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('All');
  const [openDeleteCategory, setOpenDeleteCategory] = useState(false);
  const [deleteCategoryId, setDeleteCategoryId] = useState('');

  const {
    categories,
    menuItems,
    loading,
    editingItem,
    setEditingItem,
    deleteId,
    setDeleteId,
    fetchMenu,
    handleDelete
  } = useMenuPage();

  const {
    openForm,
    openDelete,
    openCreate,
    openEdit,
    closeForm,
    openDeleteDialog,
    closeDeleteDialog
  } = useMenuDialog();

  const filteredMenu = useMenuFilter(
    menuItems,
    searchText,
    selectedCategory
  );

  const isEmptyCategory =
  selectedCategory !== 'All' &&
  menuItems.filter(i => i.categoryId === selectedCategory).length === 0;

  const handleOpenDeleteCategory = () => {
    setOpenDeleteCategory(true);
  };
  const handleConfirmDeleteCategory = async () => {
    if (!deleteCategoryId) return;

    try {
      await categoryService.delete(deleteCategoryId);

      setOpenDeleteCategory(false);
      setDeleteCategoryId('');

      setSelectedCategory('All');
      await fetchMenu();
    } catch (err) {
      console.error(err);
    }
  };

  const handleEditClick = (item) => {
    openEdit(setEditingItem, item);
  };

  const handleDeleteClick = (id) => {
    openDeleteDialog(setDeleteId, id);
  };

  const selectedItem = menuItems.find(x => x.id === deleteId);

  const confirmDelete = async () => {
    await handleDelete(deleteId);
    closeDeleteDialog();
    setDeleteId(null);
  };
  const handleToggle = async (id) => {
    try {
      await menuService.toggleStatus(id);
      await fetchMenu();
    } catch (err) {
      console.error(err);
    }
  };
  const handleDeleteCategory = async () => {
    if (!selectedCategory || selectedCategory === 'All') return;

    const confirm = window.confirm('Bạn chắc chắn muốn xoá category này?');
    if (!confirm) return;

    try {
      await categoryService.delete(selectedCategory);
      await fetchMenu();
      setSelectedCategory('All');
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <Box>
      <MenuHeader
        isAdmin={isAdmin}
        onAddClick={openCreate}
        onDeleteCategoryClick={handleDeleteCategory}
        isEmptyCategory={isEmptyCategory}
      />

      <MenuFilter
        categories={categories}
        searchText={searchText}
        onSearchChange={setSearchText}
        selectedCategory={selectedCategory}
        onCategoryChange={setSelectedCategory}
        isEmptyCategory={isEmptyCategory}
      />

      {loading
        ? <CircularProgress />
        : (
          <MenuGrid
            menuItems={filteredMenu}
            role={role}
            onEdit={handleEditClick}
            onDelete={handleDeleteClick}
            onToggle={handleToggle}
          />
        )
      }

      <Dialog open={openForm} onClose={() => closeForm(setEditingItem)}>
        <DialogTitle>
          {editingItem ? 'Edit Menu Item' : 'Create Menu Item'}
        </DialogTitle>
        <DialogContent>
          <MenuForm
            initialData={editingItem}
            onCreated={() => {
              fetchMenu();
              closeForm(setEditingItem);
            }}
          />
        </DialogContent>
      </Dialog>

      <DeleteConfirmDialog
        open={openDelete}
        onClose={closeDeleteDialog}
        onConfirm={confirmDelete}
        item={selectedItem}
      />

      <Dialog
        open={openDeleteCategory}
        onClose={() => setOpenDeleteCategory(false)}
      >
        <DialogTitle>Xoá category</DialogTitle>

        <DialogContent>
          <FormControl fullWidth sx={{ mt: 1 }}>
            <InputLabel>Chọn category</InputLabel>
            <Select
              value={deleteCategoryId}
              onChange={(e) => setDeleteCategoryId(e.target.value)}
            >
              {categories.map((c) => (
                <MenuItem key={c.id} value={c.id}>
                  {c.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setOpenDeleteCategory(false)}>
            Huỷ
          </Button>

          <Button
            color="error"
            onClick={handleConfirmDeleteCategory}
            disabled={!deleteCategoryId}
          >
            Xoá
          </Button>
        </DialogActions>
      </Dialog>
    </Box>

  );
};

export default Menu;