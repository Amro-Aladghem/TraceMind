export interface ApiSuccess<T> {
  isSuccess: true;
  data: T;
}

export interface ApiFailure {
  isSuccess: false;
  error: string;
  statusCode: number;
}

export type ApiResponse<T> = ApiSuccess<T> | ApiFailure;
