import React, { useState } from "react";
import axios from "axios";
import "bootstrap/dist/css/bootstrap.min.css";
//import "../styles/Register.css";

function RegisterForm() {
  const [step, setStep] = useState(1);
  const [role, setRole] = useState(""); // "seller" or "customer"
  const [roleError, setRoleError] = useState("");
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    phoneNumber: "",
    gender: "",
    dateOfBirth: "",
    addressLine1: "",
    addressLine2: "",
    street: "",
    city: "",
    state: "",
    country: "",
    pinCode: "",
    // Seller-specific fields
    companyName: "",
    gstin: "",
    bankAccountNumber: "",
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleNextStep = () => {
    if (!role) {
      setRoleError("Please select a role to proceed.");
      return;
    }
    setRoleError("");
    setStep(step + 1);
  };

  const validatePassword = (password) => {
    const regex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
    return regex.test(password);
  };

  const validatePhoneNumber = (phoneNumber) => {
    const regex = /^[0-9]{10}$/; // Example for a 10-digit phone number
    return regex.test(phoneNumber);
  };

  const validateDOB = (dob) => {
    const today = new Date();
    const birthDate = new Date(dob);
    const age = today.getFullYear() - birthDate.getFullYear();
    return age >= 18; // Example: Must be 18 years or older
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });

    if (name === "password" && !validatePassword(value)) {
      alert("Password must be at least 8 characters and contain a number.");
    }

    if (name === "phoneNumber" && !validatePhoneNumber(value)) {
      alert("Enter a valid 10-digit phone number.");
    }

    if (name === "dateOfBirth" && !validateDOB(value)) {
      alert("You must be at least 18 years old to register.");
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setIsSubmitting(true);

    const url =
      role === "customer"
        ? "https://localhost:7256/api/Registration/CustomerRegistration"
        : "https://localhost:7256/api/Registration/SellerRegistration";

    const dataToSubmit = role === "customer"
      ? {
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
          password: formData.password,
          phoneNumber: formData.phoneNumber,
          gender: formData.gender,
          dateOfBirth: formData.dateOfBirth,
          addressLine1: formData.addressLine1,
          addressLine2: formData.addressLine2,
          street: formData.street,
          city: formData.city,
          state: formData.state,
          country: formData.country,
          pinCode: formData.pinCode,
        }
      : {
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
          password: formData.password,
          sellerPhoneNumber: formData.phoneNumber,
          gender: formData.gender,
          companyName: formData.companyName,
          addressLine1: formData.addressLine1,
          addressLine2: formData.addressLine2,
          street: formData.street,
          city: formData.city,
          state: formData.state,
          country: formData.country,
          pinCode: formData.pinCode,
          gstin: formData.gstin,
          bankAccountNumber: formData.bankAccountNumber,
        };

    axios
      .post(url, dataToSubmit)
      .then(() => {
        alert("Registration successful!");
        window.location.href = "/"; // Redirect to homepage
      })
      .catch((err) => {
        const errorMessage =
          err.response?.data?.Message || "An error occurred. Please try again.";
        alert(errorMessage);
      })
      .finally(() => {
        setIsSubmitting(false);
      });
  };

  return (
    <div className="container py-5" style={{ background: "#f4f8fb" }}>
      <div className="progress mb-4">
        <div
          className="progress-bar bg-success"
          role="progressbar"
          style={{ width: `${(step / 3) * 100}%` }}
          aria-valuenow={step}
          aria-valuemin="1"
          aria-valuemax="3"
        />
      </div>

      {step === 1 && (
        <div className="text-center">
          <h2 className="text-success mb-4">Create Your Account</h2>
          <p className="text-dark">Are you a Seller or a Customer?</p>
          {roleError && <p className="text-danger">{roleError}</p>}
          <button
            className="btn btn-outline-success mx-2"
            onClick={() => setRole("customer")}
          >
            Customer
          </button>
          <button
            className="btn btn-outline-warning mx-2"
            onClick={() => setRole("seller")}
          >
            Seller
          </button>
          <button
            className="btn btn-primary mt-3"
            onClick={handleNextStep}
            disabled={!role}
          >
            Next
          </button>
        </div>
      )}

      {step === 2 && (
        <form className="mx-auto mt-4" style={{ maxWidth: "800px" }}>
          <h2 className="text-primary mb-3">Basic Details</h2>
          <div className="row">
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="First Name"
                name="firstName"
                value={formData.firstName}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="Last Name"
                name="lastName"
                value={formData.lastName}
                onChange={handleInputChange}
                required
              />
            </div>
          </div>
          <div className="row">
            <div className="col-md-6 mb-3">
              <input
                type="email"
                className="form-control"
                placeholder="Email"
                name="email"
                value={formData.email}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="col-md-6 mb-3">
              <input
                type="password"
                className="form-control"
                placeholder="Password"
                name="password"
                value={formData.password}
                onChange={handleInputChange}
                required
              />
            </div>
          </div>

          <button
            className="btn btn-success btn-block mt-3"
            type="button"
            onClick={handleNextStep}
          >
            Next
          </button>
        </form>
      )}

      {step === 3 && (
        <form onSubmit={handleSubmit} className="mx-auto" style={{ maxWidth: "800px" }}>
          <h2 className="text-primary mb-3">Additional Details</h2>
          <div className="row">
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="Phone Number"
                name="phoneNumber"
                value={formData.phoneNumber}
                onChange={handleInputChange}
              />
            </div>
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="Gender"
                name="gender"
                value={formData.gender}
                onChange={handleInputChange}
              />
            </div>
          </div>

          {role === "customer" && (
            <div className="row mb-3">
              <div className="col-md-12">
                <input
                  type="date"
                  className="form-control"
                  placeholder="Date of Birth"
                  name="dateOfBirth"
                  value={formData.dateOfBirth}
                  onChange={handleInputChange}
                />
              </div>
            </div>
          )}

          {role === "seller" && (
            <>
              <div className="row">
                <div className="col-md-6 mb-3">
                  <input
                    type="text"
                    className="form-control"
                    placeholder="Company Name"
                    name="companyName"
                    value={formData.companyName}
                    onChange={handleInputChange}
                  />
                </div>
                <div className="col-md-6 mb-3">
                  <input
                    type="text"
                    className="form-control"
                    placeholder="GSTIN"
                    name="gstin"
                    value={formData.gstin}
                    onChange={handleInputChange}
                  />
                </div>
              </div>
              <div className="mb-3">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Bank Account Number"
                  name="bankAccountNumber"
                  value={formData.bankAccountNumber}
                  onChange={handleInputChange}
                />
              </div>
            </>
          )}

          <div className="row">
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="Address Line 1"
                name="addressLine1"
                value={formData.addressLine1}
                onChange={handleInputChange}
              />
            </div>
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="Address Line 2"
                name="addressLine2"
                value={formData.addressLine2}
                onChange={handleInputChange}
              />
            </div>
          </div>

          <div className="row">
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="Street"
                name="street"
                value={formData.street}
                onChange={handleInputChange}
              />
            </div>
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="City"
                name="city"
                value={formData.city}
                onChange={handleInputChange}
              />
            </div>
          </div>

          <div className="row">
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="State"
                name="state"
                value={formData.state}
                onChange={handleInputChange}
              />
            </div>
            <div className="col-md-6 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="Country"
                name="country"
                value={formData.country}
                onChange={handleInputChange}
              />
            </div>
          </div>

          <div className="row">
            <div className="col-md-12 mb-3">
              <input
                type="text"
                className="form-control"
                placeholder="Pin Code"
                name="pinCode"
                value={formData.pinCode}
                onChange={handleInputChange}
              />
            </div>
          </div>

          <button
            type="submit"
            className="btn btn-success btn-block mt-3"
            disabled={isSubmitting}
          >
            {isSubmitting ? "Submitting..." : "Submit"}
          </button>
        </form>
      )}
    </div>
  );
}

export default RegisterForm;







