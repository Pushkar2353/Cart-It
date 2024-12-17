import React, { useState, useEffect } from "react";
import axios from "axios";
import "bootstrap/dist/css/bootstrap.min.css";
import { useNavigate } from "react-router-dom";


const Products = () => {
  const [products, setProducts] = useState([]);
    const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [productForm, setProductForm] = useState({
    ProductName: "",
    ProductDescription: "",
    UnitPrice: "",
    ProductStock: "",
    ProductImageUrl: "",
    CategoryId: "",
  });
  const [editingProductId, setEditingProductId] = useState(null);
  const [message, setMessage] = useState("");

  useEffect(() => {
    fetchProducts();
    fetchCategories();
  }, []);

  // Fetch products
  const fetchProducts = async () => {
    try {
      const response = await axios.get("https://localhost:7256/api/Product");
      setProducts(response.data);
    } catch (error) {
      setMessage("Error fetching products.");
      console.error("Error fetching products:", error);
    }
  };

  const fetchCategories = async () => {
    try {
      const response = await axios.get("https://localhost:7256/api/Category");
      setCategories(response.data);
    } catch (error) {
      console.error("Error fetching categories:", error);
    }
  };

  // Automatically get seller data from localStorage
  const getSellerData = () => {
    const sellerId = localStorage.getItem("sellerId");
    const jwtToken = localStorage.getItem("jwtToken");

    if (!sellerId || !jwtToken) {
      alert("Seller ID and Token are required!");
      return null; // Return null if sellerId or token is missing
    }
    return { sellerId, jwtToken };
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setProductForm({ ...productForm, [name]: value });
  };

  // Add or Update Product
  const handleSaveProduct = async (e) => {
    e.preventDefault();
  
    const sellerData = getSellerData();
    if (!sellerData) return; // If no seller data, stop the execution
  
    const { sellerId, token } = sellerData;
  
    const formData = {
      ProductName: productForm.ProductName,
      ProductDescription: productForm.ProductDescription,
      ProductPrice: productForm.UnitPrice, // Adjusted based on backend JSON
      ProductStock: productForm.ProductStock,
      ProductImageUrl: productForm.ProductImageUrl,
      CategoryId: productForm.CategoryId,
      SellerId: sellerId, // Seller ID is automatically added from localStorage
    };
  
    try {
      const config = { headers: { Authorization: `Bearer ${token}` } };
  
      if (editingProductId) {
        // Update product
        const response = await axios.put(`https://localhost:7256/api/Product/${editingProductId}`, formData, config);
        
        // Ensure the response contains the updated product data
        const updatedProduct = response.data;
  
        setProducts(
          products.map((product) =>
            product.productId === editingProductId ? { ...updatedProduct } : product
          )
        );
        setMessage("Product updated successfully.");
      } else {
        // Add product
        const response = await axios.post("https://localhost:7256/api/Product", formData, config);
        setProducts([...products, response.data]);
        setMessage("Product added successfully.");
      }
  
      resetForm();
    } catch (error) {
      setMessage(`Error ${editingProductId ? "updating" : "adding"} product.`);
      console.error("Error saving product:", error);
    }
  };
  

  const handleEditProduct = (product) => {
    // Set the editingProductId to the product's id, so you can use it later for the PUT request
    setEditingProductId(product.productId);
  
    // Set the form data to match the product being edited
    setProductForm({
      ProductName: product.productName,
      ProductDescription: product.productDescription,
      UnitPrice: product.productPrice, // Ensure the correct field name for price
      ProductStock: product.productStock,
      ProductImageUrl: product.productImageUrl,
      CategoryId: product.categoryId, // Make sure the CategoryId is part of the product object
    });
  };
  

  const handleDeleteProduct = async (productId) => {
    const sellerData = getSellerData();
    if (!sellerData) return;

    const { token } = sellerData;

    try {
      const config = { headers: { Authorization: `Bearer ${token}` } };
      await axios.delete(`https://localhost:7256/api/Product/${productId}`, config);
      setProducts(products.filter((product) => product.productId !== productId));
      setMessage("Product deleted successfully.");
    } catch (error) {
      setMessage("Error deleting product.");
      console.error("Error deleting product:", error);
    }
  };

  const resetForm = () => {
    setEditingProductId(null);
    setProductForm({
      ProductName: "",
      ProductDescription: "",
      UnitPrice: "",
      ProductStock: "",
      ProductImageUrl: "",
      CategoryId: "",
    });
  };

  return (
    <div className="product-management-page container py-5">
      <h2>Product Management</h2>
      {message && <p className="alert alert-info">{message}</p>}

      {/* Category List Section */}
      <h3>Category List</h3>
      <ul>
        {categories.length > 0 ? (
          categories.map((category) => (
            <li key={category.categoryId}>
              ID: {category.categoryId} - {category.categoryName}
            </li>
          ))
        ) : (
          <li>Loading categories...</li>
        )}
      </ul>

      {/* Product Form */}
      <form onSubmit={handleSaveProduct} className="mb-5">
        <div className="mb-3">
          <label htmlFor="ProductName" className="form-label">Product Name</label>
          <input
            type="text"
            name="ProductName"
            id="ProductName"
            className="form-control"
            value={productForm.ProductName}
            onChange={handleInputChange}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="ProductDescription" className="form-label">Product Description</label>
          <textarea
            name="ProductDescription"
            id="ProductDescription"
            className="form-control"
            value={productForm.ProductDescription}
            onChange={handleInputChange}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="UnitPrice" className="form-label">Unit Price</label>
          <input
            type="number"
            name="UnitPrice"
            id="UnitPrice"
            className="form-control"
            value={productForm.UnitPrice}
            onChange={handleInputChange}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="ProductStock" className="form-label">Stock Quantity</label>
          <input
            type="number"
            name="ProductStock"
            id="ProductStock"
            className="form-control"
            value={productForm.ProductStock}
            onChange={handleInputChange}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="CategoryId" className="form-label">Category (ID)</label>
          <input
            type="text"
            name="CategoryId"
            id="CategoryId"
            className="form-control"
            value={productForm.CategoryId}
            onChange={handleInputChange}
            required
          />
          <small className="form-text text-muted">Enter the Category ID based on the list above.</small>
        </div>
        <div className="mb-3">
          <label htmlFor="ProductImageUrl" className="form-label">Product Image URL</label>
          <input
            type="text"
            name="ProductImageUrl"
            id="ProductImageUrl"
            className="form-control"
            value={productForm.ProductImageUrl}
            onChange={handleInputChange}
            required
          />
        </div>
        <button type="submit" className="btn btn-primary">
          {editingProductId ? "Update Product" : "Add Product"}
        </button>
      </form>

      <h3>Product List</h3>
      <div className="row">
        {products.map((product) => (
          <div key={product.productId} className="col-md-4">
            <div className="card mb-4">
              <img
                src={product.productImageUrl}
                className="card-img-top"
                alt={product.productName}
              />
              <div className="card-body">
                <h5 className="card-title">{product.productName}</h5>
                <p className="card-text">{product.productDescription}</p>
                <p className="card-text">Price: ₹{product.productPrice}</p>
                <p className="card-text">Stock: {product.productStock}</p>
                <p className="card-text">
                  Category:{" "}
                  {categories.find((cat) => cat.categoryId === product.categoryId)?.categoryName ||
                    "Unknown"}
                </p>
                <button
                  className="btn btn-warning me-2"
                  onClick={() => handleEditProduct(product)}
                >
                  Edit
                </button>
                <button
                  className="btn btn-danger"
                  onClick={() => handleDeleteProduct(product.productId)}
                >
                  Delete
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="mt-auto">
        <button className="btn btn-primary" onClick={() => navigate("/seller-dashboard")}>
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

export default Products;





