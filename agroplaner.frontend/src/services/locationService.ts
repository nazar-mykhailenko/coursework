import axios from 'axios';

const API_URL = 'http://localhost:5105/api'; // adjust as needed for your backend URL

export interface Location {
  locationId: number;
  name: string;
  latitude: number;
  longitude: number;
}

export interface CreateLocationRequest {
  name: string;
  latitude: number;
  longitude: number;
}

export interface UpdateLocationRequest {
  name: string;
  latitude: number;
  longitude: number;
}

export const locationService = {
  async getLocations(): Promise<Location[]> {
    const token = localStorage.getItem('token');
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await axios.get<Location[]>(`${API_URL}/locations`, {
      headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
  },

  async getLocation(id: number): Promise<Location> {
    const token = localStorage.getItem('token');
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await axios.get<Location>(`${API_URL}/locations/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
  },

  async createLocation(data: CreateLocationRequest): Promise<Location> {
    const token = localStorage.getItem('token');
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await axios.post<Location>(`${API_URL}/locations`, data, {
      headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
  },

  async updateLocation(id: number, data: UpdateLocationRequest): Promise<Location> {
    const token = localStorage.getItem('token');
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await axios.put<Location>(`${API_URL}/locations/${id}`, data, {
      headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
  },

  async deleteLocation(id: number): Promise<void> {
    const token = localStorage.getItem('token');
    if (!token) {
      throw new Error('Authentication required');
    }

    await axios.delete(`${API_URL}/locations/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    });
  }
};
