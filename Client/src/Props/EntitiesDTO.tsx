export interface CommentDTO {
    text: string;
    reviewId: string | undefined;
    userId: string | undefined;
}

export interface ReviewDTO {
    title: string;
    text: string;
    mark: number | null;
    artworkName: string;
    groupId: number | null;
    userId: string | undefined;
    error: string;
}
