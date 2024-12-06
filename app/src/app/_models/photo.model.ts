export interface Photo {
  id: number;
  photoId?: number;
  url: string;
  isMain: boolean;
  isApproved: boolean;
  userName?: string;
}
