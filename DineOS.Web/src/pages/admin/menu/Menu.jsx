import React, { useEffect, useState } from 'react';
import { Box, CircularProgress } from '@mui/material';
import { Dialog, DialogTitle, DialogContent, Button, DialogActions } from '@mui/material';
import { useAuth } from '../../../hooks/useAuth';
import { useMenuPage } from '../../../hooks/useMenuPage';
import { useMenuFilter } from '../../../hooks/useMenuFilter';
import { useMenuDialog } from '../../../hooks/useMenuDialog';
import { menuService } from '../../../api/menuService';
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

  return (
    <Box>
      <MenuHeader
        isAdmin={isAdmin}
        onAddClick={openCreate}
      />

      <MenuFilter
        categories={categories}
        searchText={searchText}
        onSearchChange={setSearchText}
        selectedCategory={selectedCategory}
        onCategoryChange={setSelectedCategory}
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
    </Box>
  );
};

export default Menu;