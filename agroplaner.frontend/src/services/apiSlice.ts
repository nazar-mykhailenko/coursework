import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

// Define a service using a base URL and expected endpoints
export const apiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({ 
    baseUrl: '/api',
    prepareHeaders: (headers) => {
      const token = localStorage.getItem('token');
      if (token) {
        headers.set('authorization', `Bearer ${token}`);
      }
      return headers;
    },
  }),
  endpoints: (builder) => ({
    // Define endpoints here when needed
    // Example:
    // getExampleById: builder.query<ExampleResponse, string>({
    //   query: (id) => `example/${id}`,
    // }),
  }),
});

// Export hooks for usage in functional components
// export const { useGetExampleByIdQuery } = apiSlice;
