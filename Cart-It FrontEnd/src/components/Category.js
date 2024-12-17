import React, { useState, useEffect } from "react";
import axios from "axios";
import "bootstrap/dist/css/bootstrap.min.css";
import { useNavigate } from "react-router-dom";


const Category = () => {
  const [categories, setCategories] = useState([]);
      const navigate = useNavigate();
  const [newCategory, setNewCategory] = useState({ categoryName: "" });
  const [editCategory, setEditCategory] = useState(null);
  const [error, setError] = useState("");

  const apiUrl = "https://localhost:7256/api/Category"; // Replace with your actual API URL

  // Fetch all categories
  const fetchCategories = async () => {
    try {
      const response = await axios.get(apiUrl);
      setCategories(response.data);
    } catch (err) {
      setError("Failed to fetch categories");
    }
  };

  // Add new category
  const addCategory = async () => {
    if (!newCategory.categoryName) {
      setError("Category name is required");
      return;
    }

    try {
      const response = await axios.post(apiUrl, newCategory);
      setCategories([...categories, response.data]);
      setNewCategory({ categoryName: "" });
      setError("");
    } catch (err) {
      setError("Failed to add category");
    }
  };

  // Update category
  const updateCategory = async () => {
    if (!editCategory.categoryName) {
      setError("Category name is required");
      return;
    }

    try {
      const response = await axios.put(`${apiUrl}/${editCategory.categoryId}`, editCategory);
      setCategories(
        categories.map((cat) =>
          cat.categoryId === editCategory.categoryId ? response.data : cat
        )
      );
      setEditCategory(null);
      setError("");
    } catch (err) {
      setError("Failed to update category");
    }
  };

  // Delete category
  const deleteCategory = async (categoryId) => {
    try {
      await axios.delete(`${apiUrl}/${categoryId}`);
      setCategories(categories.filter((cat) => cat.categoryId !== categoryId));
    } catch (err) {
      setError("Failed to delete category");
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  return (
    <div className="container mt-5">
      <h2>Category Management</h2>

      {/* Error Message */}
      {error && <div className="alert alert-danger">{error}</div>}

      {/* Add New Category */}
      <div className="mb-4">
        <h4>Add Category</h4>
        <input
          type="text"
          className="form-control"
          placeholder="Category Name"
          value={newCategory.categoryName}
          onChange={(e) =>
            setNewCategory({ ...newCategory, categoryName: e.target.value })
          }
        />
        <button className="btn btn-primary mt-2" onClick={addCategory}>
          Add Category
        </button>
      </div>

      {/* Edit Category */}
      {editCategory && (
        <div className="mb-4">
          <h4>Edit Category</h4>
          <input
            type="text"
            className="form-control"
            placeholder="Category Name"
            value={editCategory.categoryName}
            onChange={(e) =>
              setEditCategory({ ...editCategory, categoryName: e.target.value })
            }
          />
          <button className="btn btn-success mt-2" onClick={updateCategory}>
            Save Changes
          </button>
          <button
            className="btn btn-secondary mt-2 ms-2"
            onClick={() => setEditCategory(null)}
          >
            Cancel
          </button>
        </div>
      )}

      {/* Category List */}
      <h4>All Categories</h4>
      <table className="table table-bordered">
        <thead>
          <tr>
            <th>ID</th>
            <th>Category Name</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {categories.map((category) => (
            <tr key={category.categoryId}>
              <td>{category.categoryId}</td>
              <td>{category.categoryName}</td>
              <td>
                <button
                  className="btn btn-warning btn-sm me-2"
                  onClick={() => setEditCategory(category)}
                >
                  Edit
                </button>
                <button
                  className="btn btn-danger btn-sm"
                  onClick={() => deleteCategory(category.categoryId)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div className="mt-auto">
        <button className="btn btn-primary" onClick={() => navigate("/admin-dashboard")}>
          Go To Dashboard Page
        </button>
      </div>
      <div className="mt-3">
        <button className="btn btn-primary" onClick={() => navigate("/")}>
          Go To Home Page
        </button>
      </div>
    </div>
  );
};

export default Category;
