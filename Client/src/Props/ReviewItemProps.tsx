import { Review } from "./Entities";

export interface ReviewItemProps {
  review: Review;
  loggedInUserId: string | undefined;
}