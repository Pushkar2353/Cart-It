import axios from 'axios';

const API_BASE_URL = 'https://localhost:7256/api/customer';

export const getAllCustomers = () => {
  return axios.get(`${API_BASE_URL}/GetAllCustomers`);
};

export const getCustomerById = (id) => {
  return axios.get(`${API_BASE_URL}/GetCustomerById/${id}`);
};

export const createCustomer = (customer) => {
  return axios.post(`${API_BASE_URL}/CreateCustomer`, customer);
};

export const updateCustomer = (id, customer) => {
  return axios.patch(`${API_BASE_URL}/UpdateCustomer/${id}`, customer);
};

export const deleteCustomer = (id) => {
  return axios.delete(`${API_BASE_URL}/DeleteCustomer/${id}`);
};
