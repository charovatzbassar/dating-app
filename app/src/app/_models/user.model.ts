export interface User {
  username: string;
  knownAs: string;
  gender: string;
  token: string;
  roles: string[];
  photoUrl?: string;
}
