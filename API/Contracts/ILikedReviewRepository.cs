﻿using Entities.DTOs;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILikedReviewRepository
    {
        void AddLikedReview(Guid userId, Guid reviewId);
        void RemoveUserLike(Guid userId, Guid reviewId);
        bool IsReviewLikedByUser(Guid userId, Guid reviewId);
        void MarkLikedReview(Guid userId, ReviewDTO review);
    }
}
