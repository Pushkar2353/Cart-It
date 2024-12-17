import React, { useState, useEffect } from "react";
import { ShoppingCart, Search, UserCircle, Box, Menu } from "lucide-react";
import Slider from "react-slick";
import axios from "axios";
import "../styles/HomePage.css";
import "bootstrap/dist/css/bootstrap.min.css";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { useNavigate } from "react-router-dom";

function HomePage() {
  const [user, setUser] = useState(null);
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState("All");
  const [searchTerm, setSearchTerm] = useState("");
  const [cartCount, setCartCount] = useState(0);
  const navigate = useNavigate();
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  useEffect(() => {
    const firstName = localStorage.getItem("firstName");
    const jwtToken = localStorage.getItem("jwtToken");
    const roles = JSON.parse(localStorage.getItem("roles")) || [];

    if (firstName && jwtToken) {
      setUser({ firstName, role: roles[0] });

      const savedCart = JSON.parse(localStorage.getItem("cart")) || [];
      setCartCount(savedCart.length);
    } else {
      setUser(null);
      setCartCount(0);
      localStorage.setItem("cart", JSON.stringify([]));
    }

    axios
      .get("https://localhost:7256/api/Category")
      .then((response) => {
        const categoriesData = [{ categoryId: "All", categoryName: "All" }, ...response.data];
        setCategories(categoriesData);
      })
      .catch((err) => console.log(err));
  }, []);

  useEffect(() => {
    axios
      .get(
        selectedCategory === "All"
          ? "https://localhost:7256/api/Product"
          : `https://localhost:7256/api/Product/category/${selectedCategory}`
      )
      .then((response) => setProducts(response.data))
      .catch((err) => console.log(err));
  }, [selectedCategory]);

  const handleLogout = () => {
    localStorage.removeItem("jwtToken");
    localStorage.removeItem("firstName");
    localStorage.removeItem("roles");
    setUser(null);
    setCartCount(0);
    localStorage.setItem("cart", JSON.stringify([]));
    navigate("/");
  };

  const handleCategoryChange = (categoryId) => setSelectedCategory(categoryId);
  const handleSearchChange = (e) => setSearchTerm(e.target.value);

  const handleAddToCart = async (product) => {
    const currentDate = new Date().toISOString();

    const cartItem = {
      customerId: user.customerId,
      productId: product.productId,
      productImageUrl: product.productImageUrl,
      productName: product.productName,
      cartQuantity: 1,
      amount: product.productPrice,
      createdDate: currentDate,
      updatedDate: currentDate,
    };

    try {
      const existingCart = JSON.parse(localStorage.getItem("cart")) || [];
      const existingProductIndex = existingCart.findIndex(item => item.productId === product.productId);
      if (existingProductIndex !== -1) {
        existingCart[existingProductIndex].cartQuantity += 1;
        existingCart[existingProductIndex].amount += product.productPrice;
      } else {
        existingCart.push(cartItem);
      }
      localStorage.setItem("cart", JSON.stringify(existingCart));
      setCartCount(existingCart.length);
      alert("Product added to cart.");
    } catch (error) {
      console.error("Error adding product to cart:", error);
      alert("Failed to add product to cart. Please try again.");
    }
  };

  const sliderSettings = {
    dots: true,
    infinite: true,
    speed: 500,
    slidesToShow: 4,
    slidesToScroll: 1,
    responsive: [
      { breakpoint: 1024, settings: { slidesToShow: 3, slidesToScroll: 1 } },
      { breakpoint: 768, settings: { slidesToShow: 2, slidesToScroll: 1 } },
      { breakpoint: 480, settings: { slidesToShow: 1, slidesToScroll: 1 } },
    ],
  };

  const navigateToDashboard = () => {
    if (user && user.role) {
      if (user.role === "Customer") {
        navigate("/customer-dashboard");
      } else if (user.role === "Seller") {
        navigate("/seller-dashboard");
      } else if (user.role === "Administrator") {
        navigate("/admin-dashboard");
      }
    }
  };

  return (
    <div style={{ background: "linear-gradient(to bottom, #ffffff, #f3f4f6)", minHeight: "100vh" }}>
      <header className="shadow sticky-top" style={{ background: "linear-gradient(to right, #1c92d2, #f2fcfe)", color: "#333" }}>
        <div className="container d-flex justify-content-between align-items-center py-3">
          <div className="d-flex align-items-center">
            <Box className="text-primary me-2" size={32} />
            <h1 className="mb-0" style={{ fontFamily: "Poppins, sans-serif" }}>
              Cart<span style={{ color: "#1c92d2" }}>-It</span>
            </h1>
          </div>

          <nav className="d-flex align-items-center">
            {user ? (
              <div className="me-3">
                <span className="text-dark">Hi, {user.firstName}!</span>
                <button onClick={handleLogout} className="btn btn-sm btn-outline-primary ms-2">
                  Logout
                </button>
                <button onClick={() => setIsMenuOpen(!isMenuOpen)} className="btn btn-sm btn-outline-primary ms-2">
                  <Menu size={20} />
                </button>
                {isMenuOpen && (
                  <div className="dropdown-menu show" style={{ position: "absolute", right: "0", zIndex: 10 }}>
                    <button className="dropdown-item" onClick={navigateToDashboard}>
                      Dashboard
                    </button>
                    {user.role === "Customer" && (
                      <button className="dropdown-item" onClick={() => navigate("/reviews")}>
                        Reviews
                      </button>
                    )}
                  </div>
                )}
              </div>
            ) : (
              <>
                <a href="/login" className="text-dark me-3">
                  <UserCircle className="me-1" size={20} /> Login
                </a>
                <a href="/register" className="text-dark me-3">
                  Register
                </a>
              </>
            )}
            <a href="/cart" className="text-dark position-relative">
              <ShoppingCart size={24} />
              <span className="badge bg-primary rounded-pill position-absolute top-0 start-100 translate-middle" style={{ fontSize: "0.75rem" }}>
                {cartCount}
              </span>
            </a>
          </nav>
        </div>
      </header>

      <div className="container py-4">
        <div className="input-group shadow-sm">
          <input
            type="text"
            className="form-control"
            placeholder="Search for products..."
            value={searchTerm}
            onChange={handleSearchChange}
            style={{ border: "1px solid #ddd" }}
          />
          <button className="btn btn-primary">
            <Search size={20} /> Search
          </button>
        </div>
      </div>

      <div className="container my-4">
        <Slider {...sliderSettings}>
          {categories.map((category) => (
            <div key={category.categoryId} className="text-center">
              <button
                onClick={() => handleCategoryChange(category.categoryId)}
                className={`btn btn-outline-primary ${selectedCategory === category.categoryId ? "active" : ""}`}
                style={{ width: "150px", whiteSpace: "nowrap" }}
              >
                {category.categoryName}
              </button>
            </div>
          ))}
        </Slider>
      </div>

      <div className="container">
        <Slider {...sliderSettings}>
          {products.map((product) => (
            <div key={product.productId} className="card mx-2 shadow-sm" style={{ borderRadius: "8px", minWidth: "200px" }}>
              <img
                src={product.productImageUrl}
                className="card-img-top"
                alt={product.productName}
                style={{ width: "100%", height: "200px", objectFit: "contain" }}
              />
              <div className="card-body text-center">
                <h5 className="card-title">{product.productName}</h5>
                <p className="card-text">₹{product.productPrice}</p>
                <button className="btn btn-sm btn-primary" onClick={() => handleAddToCart(product)}>
                  Add to Cart
                </button>
              </div>
            </div>
          ))}
        </Slider>
      </div>

      <footer className="text-white text-center py-3 mt-5" style={{ background: "#141e30" }}>
        <p>© 2024 Cart-It. All Rights Reserved.</p>
      </footer>
    </div>
  );
}

export default HomePage;




























