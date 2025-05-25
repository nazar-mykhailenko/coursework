import api from './api';

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
    const response = await api.get<Location[]>('/locations');
    return response.data;
  },

  async getLocation(id: number): Promise<Location> {
    const response = await api.get<Location>(`/locations/${id}`);
    return response.data;
  },

  async createLocation(data: CreateLocationRequest): Promise<Location> {
    const response = await api.post<Location>('/locations', data);
    return response.data;
  },

  async updateLocation(id: number, data: UpdateLocationRequest): Promise<Location> {
    const response = await api.put<Location>(`/locations/${id}`, data);
    return response.data;
  },

  async deleteLocation(id: number): Promise<void> {
    await api.delete(`/locations/${id}`);
  }
};
