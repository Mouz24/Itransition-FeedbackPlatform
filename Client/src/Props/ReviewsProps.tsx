export interface ReviewsProps {
    loggedInUserId: string | undefined;
    tagIds: number[];
    isLoading: boolean;
    setIsLoading: React.Dispatch<React.SetStateAction<boolean>>;
  }